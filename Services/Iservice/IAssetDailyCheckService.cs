using BMEStokYonetim.Data.Entities;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IAssetDailyCheckService
    {
        Task<bool> HasCheckTodayAsync(int assetId, string userId);
        Task<AssetDailyCheck?> GetTodayCheckAsync(int assetId, string userId);
        Task CreateOrUpdateCheckAsync(AssetDailyCheck check);
        Task<List<AssetDailyCheck>> GetChecksAsync(int assetId, DateOnly? startDate = null, DateOnly? endDate = null);
    }
}
