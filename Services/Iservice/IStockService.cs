namespace BMEStokYonetim.Services.Iservice
{
    public interface IStockService
    {
        // 🔹 Satınalma üzerinden stok girişi
        Task StockEntryAsync(int purchaseDetailId, int quantity, int warehouseId, string userId, string? docNo, string? desc);

        // 🔹 Bağımsız stok girişi (productId ile)
        Task StockEntryAsync(int productId, int quantity, string userId, string? docNo, string? desc);

        // 🔹 Stok çıkışı (talep, bakım, varlık vb.)
        Task StockExitAsync(int productId, int quantity, int sourceWarehouseId, string userId,
                            string? docNo, string? desc,
                            int? assetId = null, int? requestItemId = null,
                            int? km = null, int? hourMeter = null);

        // 🔹 Depolar arası transfer
        Task TransferAsync(int productId, int quantity, int fromWarehouseId, int toWarehouseId,
                           string userId, string? docNo, string? desc);

        // 🔹 Manuel rezervasyon (depo tarafından)
        Task ManualReserveAsync(int productId, int warehouseId, int quantity, string userId, string? note = null);

        // 🔹 Rezervasyon iptali (sadece manuel iptal edilebilir)
        Task CancelReservationAsync(int reservationId, string userId);
        Task<int> FuelInAsync(int productId, int warehouseId, int quantity, string unit, string? documentNo, string? description, string userId);
        Task<int> FuelOutAsync(int productId, int warehouseId, int quantity, string unit, string? documentNo, string? description, string userId, int? assetId, int? km, int? hourMeter);
    }

    // 🔸 Yardımcı sınıf
    public static class IStockCalc
    {
        public static TalepDurumu CalcStatusAfterEntry(int orderedQty, int totalEntered,
                                                       TalepDurumu statusApproved,
                                                       TalepDurumu statusPartial,
                                                       TalepDurumu statusClosed)
        {
            return totalEntered <= 0 ? statusApproved : totalEntered < orderedQty ? statusPartial : statusClosed;
        }
    }
}
