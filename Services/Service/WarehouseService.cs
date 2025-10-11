using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _context;

        public WarehouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===========================
        // 🔹 Yeni Eklenen Metod
        // ===========================
        /// <summary>
        /// Belirli bir depoda, belirli bir ürünün kullanılabilir (AvailableQuantity) miktarını döner.
        /// Eğer stok kaydı yoksa 0 döner.
        /// </summary>
        public async Task<int> GetAvailableQuantityAsync(int warehouseId, int productId)
        {
            WarehouseStock? stock = await _context.WarehouseStocks
                .AsNoTracking()
                .FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId);

            return stock?.AvailableQuantity ?? 0;
        }

        // ===========================
        // 🔸 Mevcut Metodlar
        // ===========================
        public async Task<int> CreateWarehouseAsync(Warehouse warehouse)
        {
            if (warehouse.Type == WarehouseType.MainDepot)
            {
                bool exists = await _context.Warehouses
                    .AnyAsync(w => w.Type == WarehouseType.MainDepot && w.Id != warehouse.Id);

                if (exists)
                {
                    throw new InvalidOperationException("Sistemde yalnızca bir Ana Depo (Main Depot) tanımlanabilir.");
                }
            }

            _ = _context.Warehouses.Add(warehouse);
            _ = await _context.SaveChangesAsync();
            return warehouse.Id;
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse)
        {
            _ = _context.Warehouses.Update(warehouse);
            _ = await _context.SaveChangesAsync();
        }

        public async Task<WarehouseStock?> GetStockAsync(int warehouseId, int productId)
        {
            return await _context.WarehouseStocks
                .AsNoTracking()
                .Include(ws => ws.Product)
                .Include(ws => ws.Warehouse)
                .FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId);
        }

        public async Task<List<WarehouseStock>> GetStocksByProductAsync(int productId)
        {
            return await _context.WarehouseStocks
                .AsNoTracking()
                .Include(ws => ws.Warehouse)
                .Where(ws => ws.ProductId == productId && ws.Quantity > 0)
                .ToListAsync();
        }

        public async Task UpdateStockLevelAsync(int warehouseId, int productId, int quantity)
        {
            WarehouseStock? stock = await _context.WarehouseStocks
                .FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId);

            if (stock == null)
            {
                if (quantity < 0)
                {
                    throw new InvalidOperationException("Yetersiz stok");
                }

                stock = new WarehouseStock
                {
                    WarehouseId = warehouseId,
                    ProductId = productId,
                    Quantity = quantity,
                    LastUpdated = DateTime.UtcNow
                };
                _ = _context.WarehouseStocks.Add(stock);
            }
            else
            {
                stock.Quantity += quantity;
                if (stock.Quantity < 0)
                {
                    throw new InvalidOperationException("Yetersiz stok");
                }

                stock.LastUpdated = DateTime.UtcNow;
            }

            _ = await _context.SaveChangesAsync();
        }

        public async Task ReserveStockAsync(int warehouseId, int productId, int quantity)
        {
            WarehouseStock? stock = await _context.WarehouseStocks
                .FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId);

            if (stock == null || stock.AvailableQuantity < quantity)
            {
                throw new InvalidOperationException("Yetersiz stok");
            }

            stock.ReservedQuantity += quantity;
            _ = await _context.SaveChangesAsync();
        }

        public async Task ReleaseReservationAsync(int warehouseId, int productId, int quantity)
        {
            WarehouseStock? stock = await _context.WarehouseStocks
                .FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId);

            if (stock == null || stock.ReservedQuantity < quantity)
            {
                throw new InvalidOperationException("Rezervasyon hatası");
            }

            stock.ReservedQuantity -= quantity;
            _ = await _context.SaveChangesAsync();
        }

        public async Task TransferStockAsync(int fromWarehouseId, int toWarehouseId, int productId, int quantity)
        {
            await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Kaynak depodan düş
                await UpdateStockLevelAsync(fromWarehouseId, productId, -quantity);

                // Hedef depoya ekle
                await UpdateStockLevelAsync(toWarehouseId, productId, quantity);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WarehouseSummary> GetWarehouseSummaryAsync(int warehouseId)
        {
            List<WarehouseStock> stocks = await _context.WarehouseStocks
                .AsNoTracking()
                .Include(ws => ws.Product)
                .Where(ws => ws.WarehouseId == warehouseId)
                .ToListAsync();

            return new WarehouseSummary
            {
                TotalProducts = stocks.Count,
                TotalQuantity = stocks.Sum(ws => ws.Quantity),
                TotalValue = 0,
                LowStockItems = stocks.Count(ws => ws.Quantity <= ws.MinStockLevel)
            };
        }

        public async Task<List<StockAlert>> GetStockAlertsAsync(int warehouseId)
        {
            List<WarehouseStock> stocks = await _context.WarehouseStocks
                .AsNoTracking()
                .Include(ws => ws.Product)
                .Where(ws => ws.WarehouseId == warehouseId &&
                            (ws.Quantity <= ws.MinStockLevel || ws.Quantity == 0))
                .ToListAsync();

            return stocks.Select(ws => new StockAlert
            {
                ProductName = ws.Product.Name,
                CurrentStock = ws.Quantity,
                MinStockLevel = ws.MinStockLevel,
                AlertType = ws.Quantity == 0 ? "OutOfStock" : "LowStock"
            }).ToList();
        }
        public async Task AddWarehouseAsync(Warehouse warehouse)
        {
            if (warehouse.Type == WarehouseType.MainDepot)
            {
                bool exists = await _context.Warehouses.AnyAsync(w => w.Type == WarehouseType.MainDepot && w.Id != warehouse.Id);
                if (exists)
                {
                    throw new InvalidOperationException("Sistemde sadece bir Ana Depo olabilir.");
                }
            }

            _ = _context.Warehouses.Add(warehouse);
            _ = await _context.SaveChangesAsync();
        }

    }
}
