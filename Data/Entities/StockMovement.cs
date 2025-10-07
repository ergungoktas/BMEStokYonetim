using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("StockMovements")]
    public class StockMovement
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        [Required]
        public MovementType MovementType { get; set; }   //  enum property

        [Required]
        public int Quantity { get; set; }

        [Required, StringLength(50)]
        public ProductUnit Unit { get; set; }

        public DateTime MovementDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? DocumentNumber { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Warehouse iliþkileri
        public int? SourceWarehouseId { get; set; }
        public Warehouse? SourceWarehouse { get; set; }

        public int? TargetWarehouseId { get; set; }
        public Warehouse? TargetWarehouse { get; set; }

        // Location iliþkileri
        public int? SourceLocationId { get; set; }
        public Location? SourceLocation { get; set; }

        public int? TargetLocationId { get; set; }
        public Location? TargetLocation { get; set; }

        // Ýlgili diðer iliþkiler
        public int? AssetId { get; set; }
        public Asset? Asset { get; set; }

        public int? MaintenanceId { get; set; }
        public Maintenance? Maintenance { get; set; }

        public int? PurchaseDetailId { get; set; }
        public PurchaseDetail? PurchaseDetail { get; set; }

        public int? RequestItemId { get; set; }
        public RequestItem? RequestItem { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Opsiyonel alanlar
        public int? Km { get; set; }
        public int? HourMeter { get; set; }

        public bool IsUrgent { get; set; } = false;
    }
}
