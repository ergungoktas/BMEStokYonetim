using System.Linq;
using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Models;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class FuelService : IFuelService
    {
        private readonly ApplicationDbContext _context;

        public FuelService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AkaryakitIstasyon>> GetStationsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AkaryakitIstasyonlar
                .AsNoTracking()
                .OrderBy(s => s.Ad)
                .ToListAsync(cancellationToken);
        }

        public async Task<AkaryakitIstasyon> SaveStationAsync(AkaryakitIstasyon station, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(station.Ad))
                throw new ArgumentException("İstasyon adı boş olamaz.", nameof(station));

            if (station.IstasyonID == 0)
            {
                await _context.AkaryakitIstasyonlar.AddAsync(station, cancellationToken);
            }
            else
            {
                AkaryakitIstasyon? existing = await _context.AkaryakitIstasyonlar
                    .FirstOrDefaultAsync(s => s.IstasyonID == station.IstasyonID, cancellationToken);

                if (existing == null)
                    throw new InvalidOperationException("Düzenlenecek istasyon bulunamadı.");

                existing.Ad = station.Ad;
                existing.Tip = station.Tip;
                _context.AkaryakitIstasyonlar.Update(existing);
                station = existing;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return station;
        }

        public async Task RecordFuelEntryAsync(int stationId, DateTime date, int quantityLitre, string? description,
                                               CancellationToken cancellationToken = default)
        {
            if (quantityLitre <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantityLitre), "Miktar sıfırdan büyük olmalıdır.");

            bool stationExists = await _context.AkaryakitIstasyonlar.AnyAsync(s => s.IstasyonID == stationId, cancellationToken);
            if (!stationExists)
                throw new InvalidOperationException("İstasyon bulunamadı.");

            AkaryakitGiris entry = new()
            {
                IstasyonID = stationId,
                Tarih = date,
                MiktarLitre = quantityLitre,
                Aciklama = description
            };

            await _context.AkaryakitGirisler.AddAsync(entry, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RecordFuelExitAsync(int stationId, string targetType, int targetId, DateTime date, int quantityLitre,
                                              string? description, int? km = null, int? hourMeter = null,
                                              CancellationToken cancellationToken = default)
        {
            if (quantityLitre <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantityLitre), "Miktar sıfırdan büyük olmalıdır.");

            bool stationExists = await _context.AkaryakitIstasyonlar.AnyAsync(s => s.IstasyonID == stationId, cancellationToken);
            if (!stationExists)
                throw new InvalidOperationException("İstasyon bulunamadı.");

            if (string.IsNullOrWhiteSpace(targetType))
                throw new ArgumentException("Hedef tipi zorunludur.", nameof(targetType));

            AkaryakitCikis exit = new()
            {
                KaynakIstasyonID = stationId,
                Tarih = date,
                MiktarLitre = quantityLitre,
                HedefTip = targetType,
                HedefID = targetId,
                KM = km,
                CalismaSaati = hourMeter,
                Aciklama = description
            };

            await _context.AkaryakitCikislar.AddAsync(exit, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<FuelMovementDto>> GetMovementsAsync(DateTime? startDate = null, DateTime? endDate = null,
                                                                   int? stationId = null, CancellationToken cancellationToken = default)
        {
            DateTime? start = startDate?.Date;
            DateTime? end = endDate?.Date;

            IQueryable<AkaryakitGiris> entryQuery = _context.AkaryakitGirisler
                .AsNoTracking()
                .Include(g => g.Istasyon)
                .OrderByDescending(g => g.Tarih);

            IQueryable<AkaryakitCikis> exitQuery = _context.AkaryakitCikislar
                .AsNoTracking()
                .Include(c => c.KaynakIstasyon)
                .OrderByDescending(c => c.Tarih);

            if (stationId.HasValue)
            {
                entryQuery = entryQuery.Where(g => g.IstasyonID == stationId.Value);
                exitQuery = exitQuery.Where(c => c.KaynakIstasyonID == stationId.Value);
            }

            if (start.HasValue)
            {
                entryQuery = entryQuery.Where(g => g.Tarih >= start.Value);
                exitQuery = exitQuery.Where(c => c.Tarih >= start.Value);
            }

            if (end.HasValue)
            {
                DateTime endInclusive = end.Value.AddDays(1).AddTicks(-1);
                entryQuery = entryQuery.Where(g => g.Tarih <= endInclusive);
                exitQuery = exitQuery.Where(c => c.Tarih <= endInclusive);
            }

            List<FuelMovementDto> entries = await entryQuery
                .Select(g => new FuelMovementDto
                {
                    MovementType = FuelMovementType.Entry,
                    RecordId = g.YakitGirisID,
                    StationId = g.IstasyonID,
                    StationName = g.Istasyon.Ad,
                    Date = g.Tarih,
                    QuantityLitre = g.MiktarLitre,
                    Description = g.Aciklama
                })
                .ToListAsync(cancellationToken);

            List<FuelMovementDto> exits = await exitQuery
                .Select(c => new FuelMovementDto
                {
                    MovementType = FuelMovementType.Exit,
                    RecordId = c.YakitCikisID,
                    StationId = c.KaynakIstasyonID,
                    StationName = c.KaynakIstasyon.Ad,
                    Date = c.Tarih,
                    QuantityLitre = c.MiktarLitre,
                    Description = c.Aciklama,
                    TargetType = c.HedefTip,
                    TargetId = c.HedefID,
                    Km = c.KM,
                    HourMeter = c.CalismaSaati
                })
                .ToListAsync(cancellationToken);

            return entries
                .Concat(exits)
                .OrderByDescending(m => m.Date)
                .ThenByDescending(m => m.RecordId)
                .ToList();
        }

        public async Task<FuelDashboardDto> GetDashboardAsync(DateTime? startDate = null, DateTime? endDate = null,
                                                              int? stationId = null, CancellationToken cancellationToken = default)
        {
            DateTime? start = startDate?.Date;
            DateTime? end = endDate?.Date;

            IQueryable<AkaryakitGiris> entryQuery = _context.AkaryakitGirisler.AsNoTracking();
            IQueryable<AkaryakitCikis> exitQuery = _context.AkaryakitCikislar.AsNoTracking();

            if (stationId.HasValue)
            {
                entryQuery = entryQuery.Where(g => g.IstasyonID == stationId.Value);
                exitQuery = exitQuery.Where(c => c.KaynakIstasyonID == stationId.Value);
            }

            if (start.HasValue)
            {
                entryQuery = entryQuery.Where(g => g.Tarih >= start.Value);
                exitQuery = exitQuery.Where(c => c.Tarih >= start.Value);
            }

            if (end.HasValue)
            {
                DateTime endInclusive = end.Value.AddDays(1).AddTicks(-1);
                entryQuery = entryQuery.Where(g => g.Tarih <= endInclusive);
                exitQuery = exitQuery.Where(c => c.Tarih <= endInclusive);
            }

            int totalEntries = await entryQuery.SumAsync(g => (int?)g.MiktarLitre, cancellationToken) ?? 0;
            int totalExits = await exitQuery.SumAsync(c => (int?)c.MiktarLitre, cancellationToken) ?? 0;
            int totalStations = stationId.HasValue
                ? 1
                : await _context.AkaryakitIstasyonlar.CountAsync(cancellationToken);

            DateTime rangeStart = start ?? DateTime.Today.AddDays(-30);
            DateTime rangeEnd = end ?? DateTime.Today;
            double totalDays = Math.Max(1, (rangeEnd.Date - rangeStart.Date).TotalDays + 1);
            decimal avgExit = totalExits == 0 ? 0 : Math.Round((decimal)totalExits / (decimal)totalDays, 2);

            List<(int StationId, int Total)> exitPerStation = (await exitQuery
                .GroupBy(c => c.KaynakIstasyonID)
                .Select(g => new { StationId = g.Key, Total = g.Sum(x => x.MiktarLitre) })
                .ToListAsync(cancellationToken))
                .Select(x => (x.StationId, x.Total))
                .ToList();

            List<(int StationId, int Total)> entryPerStation = (await entryQuery
                .GroupBy(g => g.IstasyonID)
                .Select(g => new { StationId = g.Key, Total = g.Sum(x => x.MiktarLitre) })
                .ToListAsync(cancellationToken))
                .Select(x => (x.StationId, x.Total))
                .ToList();

            FuelStationSummaryDto? busiest = null;
            if (exitPerStation.Count > 0)
            {
                (int StationId, int Total) maxExit = exitPerStation.OrderByDescending(x => x.Total).First();
                AkaryakitIstasyon? station = await _context.AkaryakitIstasyonlar.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.IstasyonID == maxExit.StationId, cancellationToken);

                if (station != null)
                {
                    int totalEntry = entryPerStation.FirstOrDefault(x => x.StationId == maxExit.StationId).Total;
                    busiest = new FuelStationSummaryDto
                    {
                        StationId = station.IstasyonID,
                        StationName = station.Ad,
                        TotalExit = maxExit.Total,
                        TotalEntry = totalEntry
                    };
                }
            }

            return new FuelDashboardDto
            {
                TotalStations = totalStations,
                TotalEntries = totalEntries,
                TotalExits = totalExits,
                AverageDailyExit = avgExit,
                BusiestStation = busiest
            };
        }
    }
}
