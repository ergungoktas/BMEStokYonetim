using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    public class Maintenance
    {
        public int Id { get; set; }

        // --- İlişkiler ---
        public int AssetId { get; set; }
        public Asset Asset { get; set; } = null!;

        public int? FaultCodeId { get; set; }
        public FaultCode? FaultCode { get; set; }

        public BakimDurumu Status { get; set; } = BakimDurumu.TalepOlusturuldu;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? WorkNotes { get; set; }

        // --- Tarihler ---
        public DateTime RequestDate { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // --- İşçilik ---
        [Column(TypeName = "decimal(18,2)")]
        public decimal LaborHours { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LaborCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        // --- Kullanıcı bilgisi ---
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }

        // --- Navigation Collections ---
        public ICollection<MaintenancePart> Parts { get; set; } = [];
        public ICollection<MaintenancePersonnel> Personnels { get; set; } = [];
    }
}
