using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("AssetCategories")]
    public class AssetCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int? ExpectedLifespan { get; set; }  // Ay veya yıl cinsinden olabilir
        public int? MaintenanceInterval { get; set; }  // Gün cinsinden bakım aralığı

        public virtual ICollection<Asset> Assets { get; set; } = [];
    }
}
