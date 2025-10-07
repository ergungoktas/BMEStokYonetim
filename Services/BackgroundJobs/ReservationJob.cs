using Quartz;

namespace BMEStokYonetim.Services.BackgroundJobs
{
    public class ReservationJob : IJob
    {
        //private readonly ILogger<ReservationJob> _logger;
        //private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        //public ReservationJob(ILogger<ReservationJob> logger, IDbContextFactory<ApplicationDbContext> contextFactory)
        //{
        //    _logger = logger;
        //    _contextFactory = contextFactory;
        //}

        //public async Task Execute(IJobExecutionContext context)
        //{
        //    _logger.LogInformation("🔹 Rezervasyon Job çalıştı: {time}", DateTime.Now);

        //    await using ApplicationDbContext db = await _contextFactory.CreateDbContextAsync();
        //    await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await db.Database.BeginTransactionAsync();

        //    try
        //    {
        //        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        //        // 🔹 Süresi dolmuş rezervasyonlar
        //        List<StockReservation> expiredReservations = await db.StockReservations
        //            .Include(r => r.Product)
        //            .Where(r => r.ExpiryDate < today && r.Status == RezervasyonDurumu.ReservationActive)
        //            .ToListAsync();

        //        if (!expiredReservations.Any())
        //        {
        //            _logger.LogInformation("⏳ Süresi dolmuş rezervasyon bulunamadı ({time})", DateTime.Now);
        //            return;
        //        }

        //        foreach (StockReservation? res in expiredReservations)
        //        {
        //            try
        //            {
        //                // 🔹 Stok güncelle
        //                WarehouseStock? warehouseStock = await db.WarehouseStocks
        //                    .FirstOrDefaultAsync(ws => ws.WarehouseId == res.WarehouseId && ws.ProductId == res.ProductId);

        //                if (warehouseStock != null)
        //                {
        //                    warehouseStock.Quantity += res.Quantity;
        //                    warehouseStock.LastUpdated = DateTime.Now;
        //                }
        //                else
        //                {
        //                    _ = db.WarehouseStocks.Add(new WarehouseStock
        //                    {
        //                        WarehouseId = res.WarehouseId,
        //                        ProductId = res.ProductId,
        //                        Quantity = res.Quantity,
        //                        LastUpdated = DateTime.Now
        //                    });
        //                }

        //                // 🔹 Stok hareketi
        //                StockMovement movement = new()
        //                {
        //                    ProductId = res.ProductId,
        //                    SourceWarehouseId = res.WarehouseId,
        //                    Quantity = res.Quantity,
        //                    MovementType = MovementType.Out, // Enum kullanımı
        //                    MovementDate = DateTime.Now,
        //                    Description = $"Rezervasyon süresi dolduğu için otomatik serbest bırakma. RezId={res.Id}",
        //                    RequestItemId = res.RequestItemId
        //                };
        //                _ = db.StockMovements.Add(movement);

        //                // 🔹 Rezervasyon güncelle
        //                res.Status = RezervasyonDurumu.ReservationExpired;

        //                _logger.LogInformation(
        //                    "✅ Rezervasyon {resId} süresi doldu. {qty} adet stok serbest bırakıldı.",
        //                    res.Id, res.Quantity);
        //            }
        //            catch (Exception innerEx)
        //            {
        //                // 🔹 Tekil rezervasyon hatası job’ı durdurmaz
        //                _logger.LogWarning(innerEx, "⚠️ Rezervasyon {resId} işlenirken hata oluştu", res.Id);
        //            }
        //        }

        //        // 🔹 Tüm değişiklikleri kaydet
        //        _ = await db.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        _logger.LogInformation("🎯 Toplam {count} rezervasyon kapatıldı ve stok serbest bırakıldı.",
        //            expiredReservations.Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "❌ Rezervasyon job çalışırken hata oluştu");
        //    }
        //    finally
        //    {
        //        await transaction.DisposeAsync();
        //        await db.DisposeAsync();
        //        _logger.LogInformation("🔚 Rezervasyon Job tamamlandı ({time})", DateTime.Now);
        //    }
        //}
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
