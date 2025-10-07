using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservationService> _logger;
        private readonly IWarehouseService _warehouseService;

        public ReservationService(ApplicationDbContext context,
                                  ILogger<ReservationService> logger,
                                  IWarehouseService warehouseService)
        {
            _context = context;
            _logger = logger;
            _warehouseService = warehouseService;
        }

        // -------------------- CREATE AUTO --------------------
        public async Task CreateAutoReservationAsync(int requestItemId, int quantity)
        {
            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                RequestItem? requestItem = await _context.RequestItems
                    .Include(ri => ri.Request)
                    .Include(ri => ri.Product)
                    .FirstOrDefaultAsync(ri => ri.Id == requestItemId);

                if (requestItem == null)
                {
                    throw new InvalidOperationException("RequestItem bulunamadı.");
                }

                Warehouse? mainWarehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.Type == WarehouseType.MainDepot);

                if (mainWarehouse == null)
                {
                    throw new InvalidOperationException("Ana depo bulunamadı.");
                }

                StockReservation reservation = new()
                {
                    RequestItemId = requestItemId,
                    ProductId = requestItem.ProductId,
                    WarehouseId = mainWarehouse.Id,
                    ReservedQuantity = quantity,
                    Type = ReservationType.Automatic,
                    Status = RezervasyonDurumu.ReservationActive,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _ = _context.StockReservations.Add(reservation);
                _ = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"[AUTO] Rezervasyon oluşturuldu: {requestItem.Product.Name} - {quantity} adet");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Otomatik rezervasyon oluşturulamadı.");
                throw;
            }
        }

        // -------------------- CREATE MANUAL --------------------
        public async Task CreateManualReservationAsync(int requestItemId, int quantity, DateOnly expiryDate)
        {
            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                RequestItem? requestItem = await _context.RequestItems
                    .Include(ri => ri.Product)
                    .FirstOrDefaultAsync(ri => ri.Id == requestItemId);

                if (requestItem == null)
                {
                    throw new InvalidOperationException("RequestItem bulunamadı.");
                }

                Warehouse? mainWarehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.Type == WarehouseType.MainDepot);

                if (mainWarehouse == null)
                {
                    throw new InvalidOperationException("Ana depo bulunamadı.");
                }

                StockReservation reservation = new()
                {
                    RequestItemId = requestItemId,
                    ProductId = requestItem.ProductId,
                    WarehouseId = mainWarehouse.Id,
                    ReservedQuantity = quantity,
                    Type = ReservationType.Manual,
                    Status = RezervasyonDurumu.ReservationActive,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    ReleasedAt = expiryDate.ToDateTime(TimeOnly.MaxValue)
                };

                _ = _context.StockReservations.Add(reservation);
                _ = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"[MANUAL] Rezervasyon oluşturuldu: {requestItem.Product.Name} - {quantity} adet (Geçerlilik: {expiryDate:dd.MM.yyyy})");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Manuel rezervasyon oluşturulamadı.");
                throw;
            }
        }

        // -------------------- COMPLETE --------------------
        public async Task CompleteReservationAsync(int reservationId)
        {
            StockReservation? reservation = await _context.StockReservations
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation != null)
            {
                reservation.Status = RezervasyonDurumu.ReservationCompleted;
                reservation.IsActive = false;
                reservation.ReleasedAt = DateTime.Now;

                _ = await _context.SaveChangesAsync();
                _logger.LogInformation($"Rezervasyon tamamlandı: #{reservation.Id}");
            }
        }

        // -------------------- CANCEL --------------------
        public async Task CancelReservationAsync(int reservationId)
        {
            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                StockReservation? reservation = await _context.StockReservations
                    .FirstOrDefaultAsync(r => r.Id == reservationId);

                if (reservation == null)
                {
                    throw new Exception("Rezervasyon bulunamadı.");
                }

                if (reservation.Type == ReservationType.Automatic)
                {
                    throw new Exception("Otomatik rezervasyonlar iptal edilemez.");
                }

                reservation.Status = RezervasyonDurumu.ReservationCancelled;
                reservation.IsActive = false;
                reservation.ReleasedAt = DateTime.Now;

                await _warehouseService.ReleaseReservationAsync(
                    reservation.WarehouseId,
                    reservation.ProductId,
                    reservation.ReservedQuantity);

                _ = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Rezervasyon iptal edildi: #{reservation.Id}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Rezervasyon iptali başarısız.");
                throw;
            }
        }

        // -------------------- GET ACTIVE --------------------
        public async Task<List<StockReservation>> GetActiveReservationsAsync()
        {
            return await _context.StockReservations
                .AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.RequestItem)
                .Include(r => r.Warehouse)
                .Where(r => r.Status == RezervasyonDurumu.ReservationActive && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // -------------------- PROCESS EXPIRED --------------------
        public async Task<ReservationProcessResult> ProcessExpiredReservationsAsync()
        {
            ReservationProcessResult result = new();
            DateTime today = DateTime.Now;

            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                List<StockReservation> expiredReservations = await _context.StockReservations
                    .Include(r => r.Product)
                    .Include(r => r.Warehouse)
                    .Where(r => r.Status == RezervasyonDurumu.ReservationActive &&
                                r.Type == ReservationType.Manual &&
                                r.ReleasedAt != null &&
                                r.ReleasedAt < today)
                    .ToListAsync();

                result.TotalProcessed = expiredReservations.Count;

                foreach (StockReservation? res in expiredReservations)
                {
                    res.Status = RezervasyonDurumu.ReservationExpired;
                    res.IsActive = false;

                    await _warehouseService.ReleaseReservationAsync(
                        res.WarehouseId,
                        res.ProductId,
                        res.ReservedQuantity);

                    result.ExpiredReservations++;
                    _logger.LogInformation($"Süresi dolan rezervasyon: #{res.Id}");
                }

                _ = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                result.Message = $"{result.ExpiredReservations} rezervasyon süresi doldu.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Rezervasyon süresi kontrolü sırasında hata.");
                result.Message = "Hata: " + ex.Message;
            }

            return result;
        }

        // -------------------- GET EXPIRING --------------------
        public async Task<List<StockReservation>> GetExpiringReservationsAsync(DateOnly date)
        {
            return await _context.StockReservations
                .AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.RequestItem)
                .Include(r => r.Warehouse)
                .Where(r => r.Status == RezervasyonDurumu.ReservationActive &&
                            r.Type == ReservationType.Manual &&
                            r.ReleasedAt.HasValue &&
                            DateOnly.FromDateTime(r.ReleasedAt.Value) == date)
                .ToListAsync();
        }
    }
}
