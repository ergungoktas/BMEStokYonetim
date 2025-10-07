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
    }

    // 🔸 Yardımcı sınıf
    public static class IStockCalc
    {
        public static TalepDurumu CalcStatusAfterEntry(int orderedQty, int totalEntered,
                                                       TalepDurumu statusApproved,
                                                       TalepDurumu statusPartial,
                                                       TalepDurumu statusClosed)
        {
            if (totalEntered <= 0)
            {
                return statusApproved;
            }
            else
            {
                return totalEntered < orderedQty ? statusPartial : statusClosed;
            }
        }
    }
}
