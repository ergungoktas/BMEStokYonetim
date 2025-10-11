using BMEStokYonetim.Data.Entities;

public interface IMaintenanceService
{
    Task<int> CreateMaintenanceAsync(Maintenance maintenance, string userId, CancellationToken cancellationToken = default);

    Task<List<Maintenance>> GetMaintenanceListAsync(CancellationToken cancellationToken = default);
    Task<Maintenance?> GetMaintenanceAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(int maintenanceId, BakimDurumu status, string userId, CancellationToken cancellationToken = default);
    Task<MaintenancePart?> AddPartAsync(int maintenanceId, int productId, int quantity, decimal unitCost, CancellationToken cancellationToken = default);
    Task RemovePartAsync(int partId, CancellationToken cancellationToken = default);
    Task<MaintenancePersonnel?> AddPersonnelAsync(int maintenanceId, string personnelName, decimal hours, decimal rate, string? userId = null, string? role = null, CancellationToken cancellationToken = default);
    Task RemovePersonnelAsync(int personnelId, CancellationToken cancellationToken = default);
    Task UpdateWorkInfoAsync(int maintenanceId, string? workNotes, DateTime? plannedDate = null, CancellationToken cancellationToken = default);
    Task<List<FaultCode>> GetFaultCodesAsync(bool onlyActive = true, CancellationToken cancellationToken = default);
    Task<FaultCode> CreateFaultCodeAsync(FaultCode faultCode, CancellationToken cancellationToken = default);
    Task UpdateFaultCodeStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default);
    Task<Maintenance?> GetMaintenanceFormAsync(int id, CancellationToken cancellationToken = default);
    Task<string?> GetAssetNameAsync(int assetId, CancellationToken cancellationToken = default);
    Task<bool> SaveMaintenanceAsync(Maintenance maintenance, CancellationToken cancellationToken = default);
    Task<Maintenance> CreateMaintenanceAsyncModel();
}
