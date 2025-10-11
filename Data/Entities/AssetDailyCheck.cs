using System.ComponentModel.DataAnnotations;

namespace BMEStokYonetim.Data.Entities
{
    public class AssetDailyCheck
    {
        public int Id { get; set; }

        // 🔹 İlişkiler
        [Required]
        public int AssetId { get; set; }
        public Asset Asset { get; set; } = default!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;

        // 🔹 Genel Bilgiler
        [Required]
        public DateOnly CheckDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public int? Km { get; set; }
        public int? HourMeter { get; set; }

        [MaxLength(500)]
        public string? GeneralNotes { get; set; }

        // 🔹 Kontrol Alanları (bool + açıklama)
        public bool? EngineOilOk { get; set; }
        [MaxLength(300)]
        public string? EngineOilNotes { get; set; }

        public bool? HydraulicOilOk { get; set; }
        [MaxLength(300)]
        public string? HydraulicOilNotes { get; set; }

        public bool? CoolantOk { get; set; }
        [MaxLength(300)]
        public string? CoolantNotes { get; set; }

        public bool? TireConditionOk { get; set; }
        [MaxLength(300)]
        public string? TireNotes { get; set; }

        public bool? LightsOk { get; set; }
        [MaxLength(300)]
        public string? LightsNotes { get; set; }

        public bool? HornOk { get; set; }
        [MaxLength(300)]
        public string? HornNotes { get; set; }

        public bool? SafetyEquipmentsOk { get; set; }
        [MaxLength(300)]
        public string? SafetyEquipmentsNotes { get; set; }

        // 🔹 Arıza bildirimi
        public bool? HasFault { get; set; } = false;
        [MaxLength(500)]
        public string? FaultDescription { get; set; }

        // 🔹 Fotoğraf (opsiyonel)
        [MaxLength(255)]
        public string? PhotoPath { get; set; }

        // 🔹 Sistemsel
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
