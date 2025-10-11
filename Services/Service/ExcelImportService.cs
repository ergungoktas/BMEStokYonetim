using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using Microsoft.AspNetCore.Components.Forms; // IBrowserFile için
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BMEStokYonetim.Services.Service
{
    public class ExcelImportService
    {
        private readonly ApplicationDbContext _context;

        public ExcelImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Blazor InputFile için overload
        public async Task<(int catCount, int assetCount)> ImportFromExcelAsync(IBrowserFile browserFile)
        {
            using MemoryStream ms = new();
            await browserFile.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024).CopyToAsync(ms);
            ms.Position = 0;
            return await ImportFromExcelAsync(ms);
        }

        // 🔹 MVC IFormFile için overload
        public async Task<(int catCount, int assetCount)> ImportFromExcelAsync(IFormFile file)
        {
            using MemoryStream ms = new();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            return await ImportFromExcelAsync(ms);
        }

        // 🔹 Ortak çekirdek
        public async Task<(int catCount, int assetCount)> ImportFromExcelAsync(Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage package = new(stream);
            ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault()
                ?? throw new InvalidOperationException("Excel dosyasında geçerli bir sayfa bulunamadı.");

            int rows = ws.Dimension.Rows;
            int newCategories = 0;
            int newAssets = 0;

            // Kategori ve Lokasyon cache’leri
            Dictionary<string, AssetCategory> existingCategories = await _context.AssetCategories
                .AsNoTracking()
                .ToDictionaryAsync(c => c.Name.Trim().ToLower(), c => c);

            Dictionary<string, Location> existingLocations = await _context.Locations
                .AsNoTracking()
                .ToDictionaryAsync(l => l.Name.Trim().ToLower(), l => l);

            // Satır satır işle
            for (int r = 2; r <= rows; r++)
            {
                string name = ws.Cells[r, 1].Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                string categoryName = ws.Cells[r, 11].Text?.Trim() ?? "Tanımsız";
                string normalizedCategory = categoryName.ToLower();

                // 🔸 Kategori bul veya ekle
                if (!existingCategories.TryGetValue(normalizedCategory, out AssetCategory? category))
                {
                    category = new AssetCategory
                    {
                        Name = categoryName.Trim(),
                        Description = "Excel'den otomatik eklendi"
                    };
                    _ = _context.AssetCategories.Add(category);
                    existingCategories[normalizedCategory] = category;
                    newCategories++;
                }

                // 🔸 Lokasyon bul veya ekle
                string locationName = ws.Cells[r, 12].Text?.Trim() ?? "Bilinmiyor";
                string normalizedLocation = locationName.ToLower();

                if (!existingLocations.TryGetValue(normalizedLocation, out Location? location))
                {
                    location = new Location
                    {
                        Name = locationName,
                        Description = "Excel'den otomatik eklendi"
                    };
                    _ = _context.Locations.Add(location);
                    existingLocations[normalizedLocation] = location;
                }

                // 🔸 Yeni Asset oluştur
                Asset asset = new()
                {
                    Name = name,
                    Brand = ws.Cells[r, 2].Text?.Trim(),
                    Model = ws.Cells[r, 3].Text?.Trim(),
                    SerialNumber = ws.Cells[r, 4].Text?.Trim(),
                    PlateNumber = ws.Cells[r, 5].Text?.Trim(),
                    CurrentKM = TryInt(ws.Cells[r, 6].Text),
                    WorkingHours = TryInt(ws.Cells[r, 7].Text),
                    ModelYear = TryInt(ws.Cells[r, 8].Text),
                    LastMaintenanceDate = TryDate(ws.Cells[r, 9].Text),
                    MaintenanceInterval = TryInt(ws.Cells[r, 10].Text),
                    Category = category,
                    Location = location,
                    Description = ws.Cells[r, 13].Text?.Trim(),
                    IsActive = TryBool(ws.Cells[r, 14].Text)
                };

                _ = _context.Assets.Add(asset);
                newAssets++;
            }

            _ = await _context.SaveChangesAsync();
            return (newCategories, newAssets);
        }

        // Helpers
        private static int? TryInt(string? v)
        {
            return int.TryParse(v, out int i) ? i : null;
        }

        private static DateTime? TryDate(string? v)
        {
            return DateTime.TryParse(v, out DateTime d) ? d : null;
        }

        private static bool TryBool(string? v)
        {
            return (v ?? "").Trim().ToLower() is "true" or "evet" or "1" or "aktif";
        }
    }
}
