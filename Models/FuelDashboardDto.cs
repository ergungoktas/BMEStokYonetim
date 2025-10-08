namespace BMEStokYonetim.Models
{
    public class FuelStationSummaryDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public int TotalExit { get; set; }
        public int TotalEntry { get; set; }
    }

    public class FuelDashboardDto
    {
        public int TotalStations { get; set; }
        public int TotalEntries { get; set; }
        public int TotalExits { get; set; }
        public int NetBalance => TotalEntries - TotalExits;
        public decimal AverageDailyExit { get; set; }
        public FuelStationSummaryDto? BusiestStation { get; set; }
    }
}
