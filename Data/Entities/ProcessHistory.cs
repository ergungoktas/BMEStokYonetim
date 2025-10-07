using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("ProcessHistories")]
    public class ProcessHistory
    {
        [Key]
        public int Id { get; set; }

        // Hangi entity için (Request, RequestItem, Purchase, PurchaseDetail, StockMovement vb.)
        [Required, StringLength(30)]
        public string EntityType { get; set; } = "Request";

        // Entity’nin Id’si (örn. Request.Id veya PurchaseDetail.Id)
        [Required]
        public int EntityId { get; set; }

        // ✅ Süreç durumu artık enum ile tutuluyor
        [Required]
        public TalepDurumu Status { get; set; }
        // Onay aşaması (birim, yönetim, vb.)
        public OnayAsamasi ApprovalStage { get; set; } = OnayAsamasi.None;

        // İşlemi yapan kullanıcı
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        // İşlemin tarihi
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
