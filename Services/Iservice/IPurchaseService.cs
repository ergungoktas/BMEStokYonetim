
using BMEStokYonetim.Data.Entities;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IPurchaseService
    {
        Task<Purchase> CreatePurchaseAsync(Purchase purchase, List<PurchaseDetail> details, string userId);
        Task ApproveByUnitAsync(int purchaseDetailId, string userId);
        Task ApproveByManagementAsync(int purchaseDetailId, string userId);
        Task RejectAsync(int purchaseDetailId, string userId);
        Task<string> GeneratePurchaseNumberAsync(int locationId);
    }
}
