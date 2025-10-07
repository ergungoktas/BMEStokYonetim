namespace BMEStokYonetim.Models
{
    public class StockReportDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string MainCategoryName { get; set; } = "";
        public string SubCategoryName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Unit { get; set; } = "";
        public int CurrentStock { get; set; }
        public int? MinStock { get; set; }
        public string Status { get; set; } = "";
    }

    public class StockSummaryDto
    {
        public int TotalProducts { get; set; }
        public int TotalItemsInStock { get; set; }
        public int LowStockItems { get; set; }
    }
}
