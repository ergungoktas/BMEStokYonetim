using BMEStokYonetim.Data;
using BMEStokYonetim.Models;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class StockReportService : IStockReportService
    {
        private readonly ApplicationDbContext _context;

        public StockReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockReportDto>> GetStockReportAsync()
        {
            List<Data.Entities.Product> products = await _context.Products
                .Include(p => p.Category)
                    .ThenInclude(sc => sc.ProductMainCategory)
                .ToListAsync(); // 🔹 Artık sorgu SQL’de bitti, bundan sonrası RAM’de çalışır

            return products.Select(p => new StockReportDto
            {
                ProductId = p.Id,
                ProductCode = p.Code,
                ProductName = p.Name,
                MainCategoryName = p.Category != null && p.Category.ProductMainCategory != null
                    ? p.Category.ProductMainCategory.Name
                    : "Yok",
                SubCategoryName = p.Category != null ? p.Category.Name : "Yok",
                Brand = p.Brand ?? "",
                Unit = p.Unit.ToString(),
                CurrentStock = p.CurrentStock,
                MinStock = p.MinStock,
                Status = p.MinStock.HasValue && p.CurrentStock <= p.MinStock.Value
                    ? "Düşük Stok"
                    : p.CurrentStock == 0 ? "Stok Yok" : "Normal"
            })
            .OrderBy(p => p.ProductName)
            .ToList();
        }


        public async Task<StockSummaryDto> GetStockSummaryAsync()
        {
            List<Data.Entities.Product> products = await _context.Products.ToListAsync();

            return new StockSummaryDto
            {
                TotalProducts = products.Count,
                TotalItemsInStock = products.Sum(p => p.CurrentStock),
                LowStockItems = products.Count(p => p.MinStock.HasValue && p.CurrentStock <= p.MinStock.Value)
            };
        }

        public async Task<(List<StockReportDto> Products, StockSummaryDto Summary)> GetStockReportWithSummaryAsync()
        {
            List<Data.Entities.Product> products = await _context.Products
                .Include(p => p.Category)
                    .ThenInclude(sc => sc.ProductMainCategory)
                .ToListAsync();

            List<StockReportDto> reportData = products.Select(p => new StockReportDto
            {
                ProductId = p.Id,
                ProductCode = p.Code,
                ProductName = p.Name,
                MainCategoryName = p.Category?.ProductMainCategory?.Name ?? "Yok",
                SubCategoryName = p.Category?.Name ?? "Yok",
                Brand = p.Brand ?? "",
                Unit = p.Unit.ToString(),
                CurrentStock = p.CurrentStock,
                MinStock = p.MinStock,
                Status = p.MinStock.HasValue && p.CurrentStock <= p.MinStock.Value ? "Düşük Stok" :
                         p.CurrentStock == 0 ? "Stok Yok" : "Normal"
            }).OrderBy(p => p.ProductName).ToList();

            StockSummaryDto summary = new()
            {
                TotalProducts = products.Count,
                TotalItemsInStock = products.Sum(p => p.CurrentStock),
                LowStockItems = products.Count(p => p.MinStock.HasValue && p.CurrentStock <= p.MinStock.Value)
            };

            return (reportData, summary);
        }
    }
}
