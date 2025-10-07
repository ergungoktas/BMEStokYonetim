
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("Purchases")]
    public class Purchase
    {
        [Key] public int Id { get; set; }
        [Required, StringLength(50)] public string PurchaseNumber { get; set; } = string.Empty;
        [Required] public DateTime PurchaseDate { get; set; } = DateTime.Now;
        [StringLength(250)] public string? Description { get; set; }
        [Required] public int LocationId { get; set; }
        [ForeignKey("LocationId")] public virtual Location Location { get; set; } = null!;
        public int? WorkplaceId { get; set; }

        public int? RequestId { get; set; }
        [ForeignKey("RequestId")] public virtual Request? Request { get; set; }
        [Required] public string CreatedByUserId { get; set; } = string.Empty;
        [ForeignKey("CreatedByUserId")] public virtual ApplicationUser CreatedByUser { get; set; } = null!;
        public virtual ICollection<PurchaseDetail> Details { get; set; } = [];
        public virtual ICollection<ProcessHistory> ProcessHistories { get; set; } = [];
    }
}
