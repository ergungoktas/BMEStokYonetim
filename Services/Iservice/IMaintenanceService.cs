using BMEStokYonetim.Data.Entities;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IMaintenanceService
    {
        Task<int> CreateMaintenanceAsync(Maintenance maintenance, string userId);

        // Artık enum kullanıyoruz
        Task UpdateStatusAsync(int maintenanceId, BakimDurumu status, string userId);

        Task AddPartAsync(int maintenanceId, int productId, int quantity, decimal unitCost, string userId);
        Task AddPersonnelAsync(int maintenanceId, string name, decimal hours, decimal rate);

        Task<Maintenance?> GetMaintenanceAsync(int id);
    }
}
