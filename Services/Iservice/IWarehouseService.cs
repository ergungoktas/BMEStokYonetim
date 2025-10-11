using BMEStokYonetim.Data.Entities;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IWarehouseService
    {
        Task<int> CreateWarehouseAsync(Warehouse warehouse);
        Task UpdateWarehouseAsync(Warehouse warehouse);

        Task<WarehouseStock?> GetStockAsync(int warehouseId, int productId);
        Task<List<WarehouseStock>> GetStocksByProductAsync(int productId);
        Task UpdateStockLevelAsync(int warehouseId, int productId, int quantity);

        Task ReserveStockAsync(int warehouseId, int productId, int quantity);
        Task ReleaseReservationAsync(int warehouseId, int productId, int quantity);

        Task TransferStockAsync(int fromWarehouseId, int toWarehouseId, int productId, int quantity);

        Task<WarehouseSummary> GetWarehouseSummaryAsync(int warehouseId);
        Task<List<StockAlert>> GetStockAlertsAsync(int warehouseId);

        // 🔹 Yeni eklendi: stok miktarını sorgulamak için
        Task<int> GetAvailableQuantityAsync(int warehouseId, int productId);
    }

    // Yardımcı DTO'lar
    public class WarehouseSummary
    {
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockItems { get; set; }
    }

    public class StockAlert
    {
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinStockLevel { get; set; }
        public string AlertType { get; set; } = string.Empty; // "LowStock", "OutOfStock"
    }
}
