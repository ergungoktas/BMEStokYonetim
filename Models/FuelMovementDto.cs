namespace BMEStokYonetim.Models
{
    public enum FuelMovementType
    {
        Entry = 1,
        Exit = 2
    }

    public class FuelMovementDto
    {
        public FuelMovementType MovementType { get; set; }
        public int RecordId { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int QuantityLitre { get; set; }
        public string? Description { get; set; }

        public string? TargetType { get; set; }
        public int? TargetId { get; set; }
        public int? Km { get; set; }
        public int? HourMeter { get; set; }

        // Ek örnek alanlar (opsiyonel)
        public int TotalEntries { get; set; }
        public int TotalExits { get; set; }
        public int NetBalance => TotalEntries - TotalExits;
    }
}
