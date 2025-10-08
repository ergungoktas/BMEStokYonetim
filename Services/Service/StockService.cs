using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProcessService _processService;
        private readonly IWarehouseService _warehouseService;

        public StockService(ApplicationDbContext context,
                            IProcessService processService,
                            IWarehouseService warehouseService)
        {
            _context = context;
            _processService = processService;
            _warehouseService = warehouseService;
        }

        // -------------------- STOK GİRİŞİ --------------------
        public async Task StockEntryAsync(int purchaseDetailId, int quantity, int warehouseId, string userId, string? docNo, string? desc)
        {
            PurchaseDetail? purchaseDetail = await _context.PurchaseDetails
                .Include(p => p.Product)
                .Include(p => p.RequestItem)
                .FirstOrDefaultAsync(p => p.Id == purchaseDetailId);

            if (purchaseDetail == null)
            {
                throw new Exception("Satınalma detayı bulunamadı.");
            }

            ProductUnit unit = purchaseDetail.Product?.Unit ?? ProductUnit.Adet;

            StockMovement movement = new()
            {
                ProductId = purchaseDetail.ProductId,
                MovementType = MovementType.In,
                Quantity = quantity,
                Unit = unit,
                MovementDate = DateTime.UtcNow,
                Description = desc,
                DocumentNumber = docNo,
                TargetWarehouseId = warehouseId,
                UserId = userId,
                PurchaseDetailId = purchaseDetailId
            };

            _ = _context.StockMovements.Add(movement);
            await _warehouseService.UpdateStockLevelAsync(warehouseId, purchaseDetail.ProductId, quantity);

            if (purchaseDetail.Product != null)
            {
                purchaseDetail.Product.CurrentStock += quantity;
            }

            int totalReceived = await _context.StockMovements
                .Where(s => s.PurchaseDetailId == purchaseDetail.Id && s.MovementType == MovementType.In)
                .SumAsync(s => s.Quantity);

            if (totalReceived < purchaseDetail.Quantity)
            {
                purchaseDetail.Status = TalepDurumu.PartialDelivery;

                if (purchaseDetail.RequestItem != null)
                {
                    purchaseDetail.RequestItem.Status = TalepDurumu.PartialDelivery;
                }
            }
            else
            {
                purchaseDetail.Status = TalepDurumu.Closed;

                if (purchaseDetail.RequestItem != null)
                {
                    purchaseDetail.RequestItem.Status = TalepDurumu.Closed;
                }
            }

            _ = await _context.SaveChangesAsync();

            await _processService.LogProcessHistoryAsync("PurchaseDetail", purchaseDetail.Id, purchaseDetail.Status, OnayAsamasi.None, userId);

            if (purchaseDetail.RequestItem != null)
            {
                await _processService.LogProcessHistoryAsync("RequestItem", purchaseDetail.RequestItem.Id, purchaseDetail.RequestItem.Status, OnayAsamasi.None, userId);
            }

            await _processService.LogProcessHistoryAsync("StockMovement", movement.Id, TalepDurumu.PurchaseApproved, OnayAsamasi.None, userId);
        }

        // -------------------- BAĞIMSIZ STOK GİRİŞİ --------------------
        public async Task StockEntryAsync(int productId, int quantity, string userId, string? docNo, string? desc)
        {
            Product? product = await _context.Products.FindAsync(productId);
            ProductUnit unit = product?.Unit ?? ProductUnit.Adet;

            Warehouse? mainWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Type == WarehouseType.MainDepot);
            if (mainWarehouse == null)
            {
                throw new Exception("Ana depo bulunamadı.");
            }

            StockMovement movement = new()
            {
                ProductId = productId,
                MovementType = MovementType.In,
                Quantity = quantity,
                Unit = unit,
                MovementDate = DateTime.UtcNow,
                Description = desc,
                DocumentNumber = docNo,
                TargetWarehouseId = mainWarehouse.Id,
                UserId = userId
            };

            _ = _context.StockMovements.Add(movement);
            await _warehouseService.UpdateStockLevelAsync(mainWarehouse.Id, productId, quantity);
            if (product != null)
            {
                product.CurrentStock += quantity;
            }
            _ = await _context.SaveChangesAsync();
        }

        // -------------------- STOK ÇIKIŞI --------------------
        public async Task StockExitAsync(int productId, int quantity, int sourceWarehouseId, string userId,
                                         string? docNo, string? desc,
                                         int? assetId = null, int? requestItemId = null,
                                         int? km = null, int? hourMeter = null)
        {
            Product? product = await _context.Products.FindAsync(productId);
            ProductUnit unit = product?.Unit ?? ProductUnit.Adet;

            WarehouseStock? sourceStock = await _warehouseService.GetStockAsync(sourceWarehouseId, productId);
            if (sourceStock == null || sourceStock.AvailableQuantity < quantity)
            {
                throw new InvalidOperationException("Seçilen depoda yeterli stok bulunmuyor.");
            }

            StockMovement movement = new()
            {
                ProductId = productId,
                MovementType = MovementType.Out,
                Quantity = quantity,
                Unit = unit,
                MovementDate = DateTime.UtcNow,
                Description = desc,
                DocumentNumber = docNo,
                SourceWarehouseId = sourceWarehouseId,
                UserId = userId,
                AssetId = assetId,
                RequestItemId = requestItemId,
                Km = km,
                HourMeter = hourMeter
            };

            _ = _context.StockMovements.Add(movement);
            await _warehouseService.UpdateStockLevelAsync(sourceWarehouseId, productId, -quantity);

            if (product != null)
            {
                product.CurrentStock = Math.Max(0, product.CurrentStock - quantity);
            }

            if (requestItemId.HasValue)
            {
                RequestItem? requestItem = await _context.RequestItems.FindAsync(requestItemId.Value);
                if (requestItem != null)
                {
                    requestItem.Status = TalepDurumu.Closed;
                    await _processService.LogProcessHistoryAsync("RequestItem", requestItem.Id, TalepDurumu.Closed, OnayAsamasi.None, userId);
                }
            }

            _ = await _context.SaveChangesAsync();
            await _processService.LogProcessHistoryAsync("StockMovement", movement.Id, TalepDurumu.Closed, OnayAsamasi.None, userId);
        }

        // -------------------- TRANSFER --------------------
        public async Task TransferAsync(int productId, int quantity, int fromWarehouseId, int toWarehouseId,
                                        string userId, string? docNo, string? desc)
        {
            Product? product = await _context.Products.FindAsync(productId);
            ProductUnit unit = product?.Unit ?? ProductUnit.Adet;

            WarehouseStock? sourceStock = await _warehouseService.GetStockAsync(fromWarehouseId, productId);
            if (sourceStock == null || sourceStock.AvailableQuantity < quantity)
            {
                throw new InvalidOperationException("Kaynak depoda yeterli stok bulunmuyor.");
            }

            StockMovement movement = new()
            {
                ProductId = productId,
                MovementType = MovementType.Transfer,
                Quantity = quantity,
                Unit = unit,
                MovementDate = DateTime.UtcNow,
                Description = desc,
                DocumentNumber = docNo,
                SourceWarehouseId = fromWarehouseId,
                TargetWarehouseId = toWarehouseId,
                UserId = userId
            };

            _ = _context.StockMovements.Add(movement);
            await _warehouseService.UpdateStockLevelAsync(fromWarehouseId, productId, -quantity);
            await _warehouseService.UpdateStockLevelAsync(toWarehouseId, productId, quantity);
            _ = await _context.SaveChangesAsync();
            await _processService.LogProcessHistoryAsync("StockMovement", movement.Id, TalepDurumu.Closed, OnayAsamasi.None, userId);
        }

        // -------------------- MANUEL REZERVASYON --------------------
        public async Task ManualReserveAsync(int productId, int warehouseId, int quantity, string userId, string? note = null)
        {
            StockReservation reservation = new()
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                ReservedQuantity = quantity,
                Type = ReservationType.Manual,
                Status = RezervasyonDurumu.ReservationActive,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ReleasedAt = DateTime.UtcNow.AddDays(1)
            };

            _ = _context.StockReservations.Add(reservation);
            _ = await _context.SaveChangesAsync();
        }

        // -------------------- REZERVASYON İPTALİ --------------------
        public async Task CancelReservationAsync(int reservationId, string userId)
        {
            StockReservation? res = await _context.StockReservations.FindAsync(reservationId);
            if (res == null)
            {
                throw new Exception("Rezervasyon bulunamadı.");
            }

            if (res.Type == ReservationType.Automatic)
            {
                throw new Exception("Otomatik rezervasyonlar iptal edilemez.");
            }

            res.IsActive = false;
            res.Status = RezervasyonDurumu.ReservationCancelled;
            res.ReleasedAt = DateTime.UtcNow;

            _ = await _context.SaveChangesAsync();
        }
    }
}
