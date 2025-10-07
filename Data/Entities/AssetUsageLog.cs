using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("AssetUsageLogs")]
    public class AssetUsageLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }
        [ForeignKey(nameof(AssetId))]
        public virtual Asset Asset { get; set; } = null!;

        [Required]
        public DateTime LogDate { get; set; } = DateTime.Now;

        // ?? Kullan�m takibi
        public int? Km { get; set; }
        public int? HourMeter { get; set; }

        // ?? �lgili stok ��k���yla ba� kurabilir
        public int? StockMovementId { get; set; }
        [ForeignKey(nameof(StockMovementId))]
        public virtual StockMovement? StockMovement { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // ��lemi yapan kullan�c�
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }


    }
}
