using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class AssetDailyCheckService : IAssetDailyCheckService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public AssetDailyCheckService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> HasCheckTodayAsync(int assetId, string userId)
        {
            await using ApplicationDbContext db = await _contextFactory.CreateDbContextAsync();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            return await db.AssetDailyChecks.AnyAsync(x => x.AssetId == assetId && x.UserId == userId && x.CheckDate == today);
        }

        public async Task<AssetDailyCheck?> GetTodayCheckAsync(int assetId, string userId)
        {
            await using ApplicationDbContext db = await _contextFactory.CreateDbContextAsync();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            return await db.AssetDailyChecks.FirstOrDefaultAsync(x => x.AssetId == assetId && x.UserId == userId && x.CheckDate == today);
        }

        public async Task CreateOrUpdateCheckAsync(AssetDailyCheck check)
        {
            await using ApplicationDbContext db = await _contextFactory.CreateDbContextAsync();
            AssetDailyCheck? existing = await db.AssetDailyChecks.FirstOrDefaultAsync(x =>
                x.AssetId == check.AssetId && x.UserId == check.UserId && x.CheckDate == check.CheckDate);

            if (existing == null)
            {
                _ = db.AssetDailyChecks.Add(check);
            }
            else
            {
                existing.Km = check.Km;
                existing.HourMeter = check.HourMeter;
                existing.EngineOilOk = check.EngineOilOk;
                existing.EngineOilNotes = check.EngineOilNotes;
                existing.HydraulicOilOk = check.HydraulicOilOk;
                existing.HydraulicOilNotes = check.HydraulicOilNotes;
                existing.CoolantOk = check.CoolantOk;
                existing.CoolantNotes = check.CoolantNotes;
                existing.TireConditionOk = check.TireConditionOk;
                existing.TireNotes = check.TireNotes;
                existing.LightsOk = check.LightsOk;
                existing.LightsNotes = check.LightsNotes;
                existing.HornOk = check.HornOk;
                existing.HornNotes = check.HornNotes;
                existing.SafetyEquipmentsOk = check.SafetyEquipmentsOk;
                existing.SafetyEquipmentsNotes = check.SafetyEquipmentsNotes;
                existing.HasFault = check.HasFault;
                existing.FaultDescription = check.FaultDescription;
                existing.PhotoPath = check.PhotoPath;
                existing.UpdatedAt = DateTime.Now;
            }

            _ = await db.SaveChangesAsync();
        }

        public async Task<List<AssetDailyCheck>> GetChecksAsync(int assetId, DateOnly? startDate = null, DateOnly? endDate = null)
        {
            await using ApplicationDbContext db = await _contextFactory.CreateDbContextAsync();
            IQueryable<AssetDailyCheck> query = db.AssetDailyChecks
                .Include(x => x.User)
                .Include(x => x.Asset)
                .Where(x => x.AssetId == assetId)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.CheckDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.CheckDate <= endDate.Value);
            }

            return await query.OrderByDescending(x => x.CheckDate).ToListAsync();
        }
    }
}
