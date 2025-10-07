using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("MaintenanceParts")]
    public class MaintenancePart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaintenanceId { get; set; }
        [ForeignKey(nameof(MaintenanceId))]
        public Maintenance Maintenance { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitCost { get; set; }

        public decimal TotalCost => Quantity * UnitCost; // ❌ sadece getter

        // Stok hareketi ile bağlama
        public int? StockMovementId { get; set; }
        public StockMovement? StockMovement { get; set; }
    }
}
