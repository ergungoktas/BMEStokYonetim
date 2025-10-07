namespace BMEStokYonetim.Services.Iservice
{
    public interface IAssetService
    {
        Task LogUsageAsync(int assetId, int? km, int? hourMeter, string userId, string? notes);
        Task SendToExternalRepairAsync(int assetId, string companyName, string? description, string userId);
        Task ReturnFromExternalRepairAsync(int repairId, string userId);
    }

}
