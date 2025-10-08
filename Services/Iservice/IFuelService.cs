using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Models;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IFuelService
    {
        Task<List<Warehouse>> GetStationsAsync();                 // İstasyon = Depo
        Task<Warehouse?> GetStationAsync(int id);
        Task<Warehouse> CreateOrUpdateStationAsync(Warehouse w);
        Task DeleteStationAsync(int id);

        Task RecordFuelEntryAsync(int stationId, DateTime date, int quantityLitre, string? description,
                                  CancellationToken cancellationToken = default);

        Task RecordFuelExitAsync(int stationId, string targetType, int targetId, DateTime date, int quantityLitre,
                                 string? description, int? km = null, int? hourMeter = null,
                                 CancellationToken cancellationToken = default);

        Task<List<FuelMovementDto>> GetMovementsAsync(DateTime? startDate = null, DateTime? endDate = null,
                                                      int? stationId = null, CancellationToken cancellationToken = default);

        Task<FuelDashboardDto> GetDashboardAsync(DateTime? startDate = null, DateTime? endDate = null,
                                                 int? stationId = null, CancellationToken cancellationToken = default);
    }
}
