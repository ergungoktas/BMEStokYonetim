using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("AssetExternalRepairs")]
    public class AssetExternalRepair
    {
        [Key] public int Id { get; set; }

        [Required] public int AssetId { get; set; }
        [ForeignKey(nameof(AssetId))] public Asset Asset { get; set; } = null!;

        [Required, StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime? SentDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
        [StringLength(50)] public string Status { get; set; } = "Sent";
        [Required] public string CreatedByUserId { get; set; } = string.Empty;
        [ForeignKey(nameof(CreatedByUserId))] public ApplicationUser CreatedByUser { get; set; } = null!;
    }
}
