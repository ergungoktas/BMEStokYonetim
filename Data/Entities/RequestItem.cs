using System.ComponentModel.DataAnnotations;

namespace BMEStokYonetim.Data.Entities
{
    public class RequestItem
    {
        public int Id { get; set; }

        // --- Ana Request ---
        public int RequestId { get; set; }
        public Request Request { get; set; } = null!;
        [Required, StringLength(50)] public string RequestItemNumber { get; set; } = string.Empty;

        // --- Ürün ---
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;   // ✅ navigation eksikse ekle

        // --- Durum (Artık Enum)
        public TalepDurumu Status { get; set; } = TalepDurumu.Open;
        // Onay aşaması (birim, yönetim, vb.)
        public OnayAsamasi ApprovalStage { get; set; } = OnayAsamasi.None;



        // --- Satınalma Detayı (opsiyonel) ---
        public int? PurchaseDetailId { get; set; }
        public PurchaseDetail? PurchaseDetail { get; set; }

        // --- Hedef Depo (opsiyonel) ---
        public int? TargetWarehouseId { get; set; }
        public Warehouse? TargetWarehouse { get; set; }

        // --- Diğer Alanlar ---
        public int RequestedQuantity { get; set; }
        public string? SequenceNo { get; set; }
        public string? RequestItemNr { get; set; }
        public string? Description { get; set; }

        // --- İlişkiler ---
        public ICollection<StockReservation> Reservations { get; set; } = [];
    }
}
