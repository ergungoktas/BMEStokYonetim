using System.ComponentModel.DataAnnotations;

namespace BMEStokYonetim.Data.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public WarehouseType Type { get; set; }   // ✅ Artık enum

        public int? LocationId { get; set; }
        public Location? Location { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<WarehouseStock> Stocks { get; set; } = [];
        public ICollection<StockMovement> SourceStockMovements { get; set; } = [];
        public ICollection<StockMovement> TargetStockMovements { get; set; } = [];
    }
}
