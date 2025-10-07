
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{

    [Table("Assets")]
    public class Asset
    {
        [Key] public int Id { get; set; }

        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(50)] public string? Brand { get; set; }
        [StringLength(50)] public string? Model { get; set; }
        [StringLength(50)] public string? SerialNumber { get; set; }
        [StringLength(20)] public string? PlateNumber { get; set; }
        public int? CurrentKM { get; set; }
        public int? WorkingHours { get; set; }
        public int? ModelYear { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public int? MaintenanceInterval { get; set; }
        [Required] public int CategoryId { get; set; }
        [ForeignKey("CategoryId")] public virtual AssetCategory Category { get; set; } = null!;
        [Required, StringLength(100)] public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Maintenance> Maintenances { get; set; } = [];
    }
}
