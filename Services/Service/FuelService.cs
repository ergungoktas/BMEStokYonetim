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

        // ===========================
        // İSTASYON (DEPO) CRUD
        // ===========================
        public async Task<List<Warehouse>> GetStationsAsync()
        {
            // İstersen burada TypeId filtreleyebilirsin (yakıt deposu tipine göre)
            return await _context.Warehouses
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .ToListAsync();
        }

        public async Task<Warehouse?> GetStationAsync(int id)
        {
            return await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Warehouse> CreateOrUpdateStationAsync(Warehouse w)
        {
            if (w.Id == 0)
            {
                _ = await _context.Warehouses.AddAsync(w);
            }
            else
            {
                Warehouse entity = await _context.Warehouses.FirstAsync(x => x.Id == w.Id);
                entity.Code = w.Code;
                entity.Name = w.Name;
                entity.IsActive = w.IsActive;
                // NOT: Warehouse sınıfında Description/TypeId/ParentWarehouseId olmayabilir.
                // Bu projede hataya sebep olduğu için dokunmuyoruz.
            }

            _ = await _context.SaveChangesAsync();
            return w;
        }

        public async Task DeleteStationAsync(int id)
        {
            Warehouse? entity = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return;
            }

            _ = _context.Warehouses.Remove(entity);
            _ = await _context.SaveChangesAsync();
        }

        // ===========================
        // YAKIT GİRİŞ / ÇIKIŞ (ESKİ İMZALAR)
        // ===========================
        public async Task RecordFuelEntryAsync(
            int stationId,
            DateTime date,
            int quantityLitre,
            string? description,
            CancellationToken cancellationToken = default)
        {
            if (quantityLitre <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantityLitre), "Miktar sıfırdan büyük olmalıdır.");
            }

            Warehouse? warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == stationId, cancellationToken);
            if (warehouse == null)
            {
                throw new InvalidOperationException("Depo (istasyon) bulunamadı.");
            }

            Product? product = await GetDefaultFuelProductAsync(cancellationToken);
            if (product == null)
            {
                throw new InvalidOperationException("Yakıt ürünü yapılandırılmamış. Lütfen en az bir yakıt ürünü tanımlayın.");
            }

            StockMovement movement = new()
            {
                ProductId = product.Id,
                Quantity = quantityLitre,
                MovementDate = date,
                Description = description,
                SourceWarehouseId = null,
                TargetWarehouseId = stationId
            };
            SetMovementUnitFlexible(movement, product);

            _ = await _context.StockMovements.AddAsync(movement, cancellationToken);

            WarehouseStock? ws = await _context.Set<WarehouseStock>()
                .FirstOrDefaultAsync(s => s.WarehouseId == stationId && s.ProductId == product.Id, cancellationToken);

            if (ws == null)
            {
                ws = new WarehouseStock
                {
                    WarehouseId = stationId,
                    ProductId = product.Id,
                    Quantity = 0,
                    ReservedQuantity = 0,
                    LastUpdated = DateTime.Now
                };
                _ = await _context.AddAsync(ws, cancellationToken);
            }

            ws.Quantity += quantityLitre;
            ws.LastUpdated = DateTime.Now;

            _ = await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RecordFuelExitAsync(
            int stationId,
            string targetType,   // Artık kullanılmıyor; geriye dönük uyumluluk için tutuluyor
            int targetId,        // Artık kullanılmıyor; geriye dönük uyumluluk için tutuluyor
            DateTime date,
            int quantityLitre,
            string? description,
            int? km = null,
            int? hourMeter = null,
            CancellationToken cancellationToken = default)
        {
            if (quantityLitre <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantityLitre), "Miktar sıfırdan büyük olmalıdır.");
            }

            Warehouse? warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == stationId, cancellationToken);
            if (warehouse == null)
            {
                throw new InvalidOperationException("Depo (istasyon) bulunamadı.");
            }

            Product? product = await GetDefaultFuelProductAsync(cancellationToken);
            if (product == null)
            {
                throw new InvalidOperationException("Yakıt ürünü yapılandırılmamış. Lütfen en az bir yakıt ürünü tanımlayın.");
            }

            WarehouseStock? ws = await _context.Set<WarehouseStock>()
                .FirstOrDefaultAsync(s => s.WarehouseId == stationId && s.ProductId == product.Id, cancellationToken);

            int available = ws?.AvailableQuantity ?? 0;
            if (available < quantityLitre)
            {
                throw new InvalidOperationException($"Yetersiz stok. Mevcut: {available}");
            }

            StockMovement movement = new()
            {
                ProductId = product.Id,
                Quantity = quantityLitre,
                MovementDate = date,
                Description = description,
                SourceWarehouseId = stationId,
                TargetWarehouseId = null,
                Km = km,
                HourMeter = hourMeter
            };
            SetMovementUnitFlexible(movement, product);

            _ = await _context.StockMovements.AddAsync(movement, cancellationToken);

            if (ws == null)
            {
                throw new InvalidOperationException("Depoda ürün kaydı bulunamadı.");
            }

            ws.Quantity -= quantityLitre;
            ws.LastUpdated = DateTime.Now;

            _ = await _context.SaveChangesAsync(cancellationToken);
        }

        // ===========================
        // RAPOR / DASHBOARD
        // ===========================
        public async Task<List<FuelMovementDto>> GetMovementsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? warehouseId = null,
            CancellationToken cancellationToken = default)
        {
            DateTime? start = startDate?.Date;
            DateTime? end = endDate?.Date;

            IQueryable<StockMovement> q = _context.StockMovements
                .AsNoTracking()
                .Include(m => m.Product)
                .Include(m => m.SourceWarehouse)
                .Include(m => m.TargetWarehouse)
                .AsQueryable();

            if (start.HasValue)
            {
                q = q.Where(m => m.MovementDate >= start.Value);
            }

            if (end.HasValue)
            {
                DateTime endInclusive = end.Value.AddDays(1).AddTicks(-1);
                q = q.Where(m => m.MovementDate <= endInclusive);
            }

            if (warehouseId.HasValue)
            {
                q = q.Where(m => m.SourceWarehouseId == warehouseId.Value || m.TargetWarehouseId == warehouseId.Value);
            }

            List<StockMovement> list = await q
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync(cancellationToken);

            List<FuelMovementDto> rows = list.Select(m => new FuelMovementDto
            {
                MovementType = (m.TargetWarehouseId != null && m.SourceWarehouseId == null)
                                ? FuelMovementType.Entry
                                : FuelMovementType.Exit,
                RecordId = m.Id,
                StationId = m.TargetWarehouseId ?? m.SourceWarehouseId ?? 0,
                StationName = (m.TargetWarehouse?.Name ?? m.SourceWarehouse?.Name) ?? "-",
                Date = m.MovementDate,
                QuantityLitre = m.Quantity,
                Description = m.Description,
                TargetType = null, // yeni modelde opsiyonel
                TargetId = null,
                Km = m.Km,
                HourMeter = m.HourMeter
            }).ToList();

            return rows;
        }

        public async Task<FuelDashboardDto> GetDashboardAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? warehouseId = null,
            CancellationToken cancellationToken = default)
        {
            DateTime? start = startDate?.Date;
            DateTime? end = endDate?.Date;

            IQueryable<StockMovement> q = _context.StockMovements.AsNoTracking();

            if (warehouseId.HasValue)
            {
                q = q.Where(m => m.SourceWarehouseId == warehouseId.Value || m.TargetWarehouseId == warehouseId.Value);
            }

            if (start.HasValue)
            {
                q = q.Where(m => m.MovementDate >= start.Value);
            }

            if (end.HasValue)
            {
                DateTime endInclusive = end.Value.AddDays(1).AddTicks(-1);
                q = q.Where(m => m.MovementDate <= endInclusive);
            }

            int totalEntries = await q.Where(m => m.TargetWarehouseId != null && m.SourceWarehouseId == null)
                                      .SumAsync(m => (int?)m.Quantity, cancellationToken) ?? 0;

            int totalExits = await q.Where(m => m.SourceWarehouseId != null && m.TargetWarehouseId == null)
                                    .SumAsync(m => (int?)m.Quantity, cancellationToken) ?? 0;

            int totalStations = warehouseId.HasValue
                ? 1
                : await _context.Warehouses.CountAsync(cancellationToken);

            DateTime rangeStart = start ?? DateTime.Today.AddDays(-30);
            DateTime rangeEnd = end ?? DateTime.Today;
            double totalDays = Math.Max(1, (rangeEnd.Date - rangeStart.Date).TotalDays + 1);
            decimal avgExit = totalExits == 0 ? 0 : Math.Round(totalExits / (decimal)totalDays, 2);

            var exitPerWarehouse = await q.Where(m => m.SourceWarehouseId != null && m.TargetWarehouseId == null)
                .GroupBy(m => m.SourceWarehouseId!.Value)
                .Select(g => new { WarehouseId = g.Key, Total = g.Sum(x => x.Quantity) })
                .ToListAsync(cancellationToken);

            var entryPerWarehouse = await q.Where(m => m.TargetWarehouseId != null && m.SourceWarehouseId == null)
                .GroupBy(m => m.TargetWarehouseId!.Value)
                .Select(g => new { WarehouseId = g.Key, Total = g.Sum(x => x.Quantity) })
                .ToListAsync(cancellationToken);

            FuelStationSummaryDto? busiest = null;
            if (exitPerWarehouse.Count > 0)
            {
                var maxExit = exitPerWarehouse.OrderByDescending(x => x.Total).First();
                Warehouse? station = await _context.Warehouses.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.Id == maxExit.WarehouseId, cancellationToken);

                if (station != null)
                {
                    int totalEntry = entryPerWarehouse.FirstOrDefault(x => x.WarehouseId == maxExit.WarehouseId)?.Total ?? 0;
                    busiest = new FuelStationSummaryDto
                    {
                        StationId = station.Id,
                        StationName = station.Name,
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

        // ===========================
        // YARDIMCILAR
        // ===========================
        private async Task<Product?> GetDefaultFuelProductAsync(CancellationToken ct)
        {
            // Yakıt ürünü seçim stratejisi:
            // 1) Kategori adı "Yakıt" / "Fuel" içeren aktif ürün
            IQueryable<Product> q = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            Product? candidate = await q.FirstOrDefaultAsync(
                p => p.Category != null && (
                        EF.Functions.Like(p.Category.Name, "%YAKIT%") ||
                        EF.Functions.Like(p.Category.Name, "%FUEL%"))
                     , ct);

            if (candidate != null)
            {
                return candidate;
            }

            // 2) Ürün adı "Mazot/Dizel/Benzin/Fuel" içeren ilk aktif ürün
            candidate = await q.FirstOrDefaultAsync(
                p => EF.Functions.Like(p.Name, "%MAZOT%") ||
                     EF.Functions.Like(p.Name, "%DIZEL%") ||
                     EF.Functions.Like(p.Name, "%DİZEL%") ||
                     EF.Functions.Like(p.Name, "%BENZIN%") ||
                     EF.Functions.Like(p.Name, "%BENZİN%") ||
                     EF.Functions.Like(p.Name, "%FUEL%"), ct);

            if (candidate != null)
            {
                return candidate;
            }

            // 3) Hiçbiri yoksa, ilk aktif ürünü kullan (konfigürasyon yapılana kadar)
            return await q.OrderBy(p => p.Id).FirstOrDefaultAsync(ct);
        }

        private static void SetMovementUnitFlexible(StockMovement movement, Product product)
        {
            System.Reflection.PropertyInfo? prop = typeof(StockMovement).GetProperty("Unit");
            if (prop == null)
            {
                return;
            }

            Type t = prop.PropertyType;
            if (t == typeof(string))
            {
                prop.SetValue(movement, product.Unit.ToString());
            }
            else
            {
                prop.SetValue(movement, product.Unit);
            }
        }
    }
}
