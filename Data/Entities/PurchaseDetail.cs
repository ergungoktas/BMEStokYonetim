using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("PurchaseDetails")]
    public class PurchaseDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PurchaseId { get; set; }

        [ForeignKey(nameof(PurchaseId))]
        public Purchase Purchase { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; } = null!;

        // --- Enum tabanlı durum ---
        [Required]
        [Column(TypeName = "int")]
        public TalepDurumu Status { get; set; } = TalepDurumu.PurchasePending;

        // --- Onay aşaması (birim, yönetim, vb.) ---
        [Required]
        [Column(TypeName = "int")]
        public OnayAsamasi ApprovalStage { get; set; } = OnayAsamasi.None;

        // --- RequestItem bağlantısı (opsiyonel) ---
        public int? RequestItemId { get; set; }

        [ForeignKey(nameof(RequestItemId))]
        public RequestItem? RequestItem { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // --- Para birimi (Enum -> int olarak saklanır) ---
        [Required]
        [Column(TypeName = "int")]
        public CurrencyType Currency { get; set; } = CurrencyType.TRY;

        public DateTime? DeliveryDate { get; set; }

        // --- İlişkiler ---
        public ICollection<StockMovement> StockMovements { get; set; } = [];

        // --- Hesaplanan property ---
        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
