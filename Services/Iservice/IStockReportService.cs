using BMEStokYonetim.Models;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IStockReportService
    {
        // 🔹 Ürün bazlı stok raporu (detaylı)
        Task<List<StockReportDto>> GetStockReportAsync();

        // 🔹 Genel stok özeti
        Task<StockSummaryDto> GetStockSummaryAsync();

        // 🔹 Rapor + özet birlikte
        Task<(List<StockReportDto> Products, StockSummaryDto Summary)> GetStockReportWithSummaryAsync();
    }
}
