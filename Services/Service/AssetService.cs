using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class AssetService : IAssetService
    {
        private readonly ApplicationDbContext _context;

        public AssetService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kullanım kaydı
        public async Task LogUsageAsync(int assetId, int? km, int? hourMeter, string userId, string? notes)
        {
            AssetUsageLog usage = new()
            {
                AssetId = assetId,
                Km = km,
                HourMeter = hourMeter,

                UserId = userId,
                Notes = notes,
                LogDate = DateTime.Now
            };

            _ = _context.AssetUsageLogs.Add(usage);
            _ = await _context.SaveChangesAsync();
        }

        // Dış servise gönderim
        public async Task SendToExternalRepairAsync(int assetId, string companyName, string? description, string userId)
        {
            AssetExternalRepair repair = new()
            {
                AssetId = assetId,
                CompanyName = companyName,      // ✅ Entity alanı
                Description = description,
                SentDate = DateTime.Now,        // ✅ Entity alanı
                CreatedByUserId = userId,       // ✅ Entity alanı
                Status = "Sent"
            };

            _ = _context.AssetExternalRepairs.Add(repair);
            _ = await _context.SaveChangesAsync();
        }

        // Dış servisten dönüş
        public async Task ReturnFromExternalRepairAsync(int repairId, string userId)
        {
            AssetExternalRepair? repair = await _context.AssetExternalRepairs.FirstOrDefaultAsync(r => r.Id == repairId);
            if (repair == null)
            {
                throw new InvalidOperationException("Kayıt bulunamadı.");
            }

            repair.ReturnDate = DateTime.Now;
            repair.Status = "Returned";
            repair.CreatedByUserId = userId;   // ✅ Kullanıcı güncellemesi

            _ = await _context.SaveChangesAsync();
        }
    }
}
