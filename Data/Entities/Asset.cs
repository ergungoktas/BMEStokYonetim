using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("Assets")]
    public class Asset
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Brand { get; set; }

        [StringLength(50)]
        public string? Model { get; set; }

        [StringLength(50)]
        public string? SerialNumber { get; set; }

        [StringLength(20)]
        public string? PlateNumber { get; set; }

        public int? CurrentKM { get; set; }

        public int? WorkingHours { get; set; }

        public int? ModelYear { get; set; }

        public DateTime? LastMaintenanceDate { get; set; }

        public int? MaintenanceInterval { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual AssetCategory Category { get; set; } = null!;

        // ✅ Eski string Location kaldırıldı
        // ✅ Foreign key olarak Location tablosuna bağlandı
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }  // 👈 EKLENDİ

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Maintenance> Maintenances { get; set; } = [];
    }
}
