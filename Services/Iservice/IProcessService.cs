using BMEStokYonetim.Data.Entities;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IProcessService
    {
        Task SetRequestItemStatusAsync(int requestItemId, TalepDurumu newStatus, OnayAsamasi approvalStage, string? userId, bool log = true);
        Task LogProcessHistoryAsync(string entityType, int entityId, TalepDurumu status, OnayAsamasi approvalStage, string? userId);
        Task<List<ProcessHistory>> GetProcessHistoryAsync(string entityType, int entityId);
        Task ApproveByUnitAsync(int requestItemId, string? userId);
        Task ApproveByManagementAsync(int requestItemId, string? userId);
        Task RejectAsync(int requestItemId, string? userId);
        Task LogAsync(string entityType, int entityId, TalepDurumu status, string? userId); // eski uyumluluk
    }
}
