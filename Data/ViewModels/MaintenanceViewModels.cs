using System.ComponentModel.DataAnnotations;

namespace BMEStokYonetim.Data.ViewModels
{
    // ----------------------------
    // 1️⃣ BAKIM FORM MODELİ
    // ----------------------------
    public class MaintenanceFormModel
    {
        [Required]
        [Display(Name = "Varlık (Ekipman/Araç)")]
        public int AssetId { get; set; }

        [Display(Name = "Arıza Kodu")]
        public int? FaultCodeId { get; set; }

        [Required]
        [Display(Name = "Bakım Türü")]
        public MaintenanceType MaintenanceType { get; set; }

        [Display(Name = "Talep Tarihi")]
        public DateTime? RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "Planlanan Tarih")]
        public DateTime? PlannedDate { get; set; }

        [Display(Name = "Başlama Tarihi")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Gerçekleşen KM / Saat")]
        public int? ActualReading { get; set; }

        [Display(Name = "Bakımı Yapan Kişi / Firma")]
        [MaxLength(150)]
        public string? PerformedBy { get; set; }

        [Display(Name = "Açıklama")]
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Toplam Maliyet")]
        public decimal TotalCost { get; set; }

        // Alt listeler
        public List<MaintenancePartInputModel> Parts { get; set; } = [];
        public List<MaintenancePersonnelInputModel> Personnel { get; set; } = [];
    }

    // ----------------------------
    // 2️⃣ BAKIM PARÇA MODELİ
    // ----------------------------
    public class MaintenancePartInputModel
    {
        [Required]
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Miktar")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1 veya daha fazla olmalıdır.")]
        public int Quantity { get; set; }

        [Display(Name = "Birim")]
        public ProductUnit Unit { get; set; }

        [Display(Name = "Birim Fiyatı (₺)")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat negatif olamaz.")]
        public decimal UnitCost { get; set; }

        [Display(Name = "Toplam Tutar (₺)")]
        public decimal TotalCost => UnitCost * Quantity;

        [Display(Name = "Açıklama")]
        [MaxLength(250)]
        public string? Description { get; set; }
    }

    // ----------------------------
    // 3️⃣ BAKIM PERSONEL MODELİ
    // ----------------------------
    public class MaintenancePersonnelInputModel
    {
        [Required]
        [Display(Name = "Personel Adı")]
        [MaxLength(150)]
        public string PersonnelName { get; set; } = string.Empty;

        [Display(Name = "Görev / Ünvan")]
        [MaxLength(100)]
        public string? Role { get; set; }

        [Display(Name = "Çalışma Süresi (saat)")]
        [Range(0.1, 9999, ErrorMessage = "Süre 0'dan büyük olmalıdır.")]
        public double HoursWorked { get; set; }

        [Display(Name = "Saatlik Ücret (₺)")]
        [Range(0, double.MaxValue, ErrorMessage = "Ücret negatif olamaz.")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Toplam Ücret (₺)")]
        public decimal TotalCost => (decimal)HoursWorked * HourlyRate;

        [Display(Name = "Açıklama")]
        [MaxLength(250)]
        public string? Notes { get; set; }
    }

    // ----------------------------
    // 4️⃣ BAKIM LİSTE MODELİ
    // ----------------------------
    public class MaintenanceListViewModel
    {
        public int Id { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string? FaultCodeName { get; set; }
        public MaintenanceType MaintenanceType { get; set; }
        public string MaintenanceTypeName => MaintenanceType.ToString();
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Cost { get; set; }
        public bool IsCompleted => EndDate.HasValue;
    }

    // ----------------------------
    // 5️⃣ ARIZA KODU MODELİ
    // ----------------------------
    public class FaultCodeInputModel
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "Kod")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Display(Name = "Arıza Adı")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Kategori")]
        public FaultCategory? Category { get; set; }

        [MaxLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }

    // ----------------------------
    // 6️⃣ ARIZA KODU LİSTE MODELİ
    // ----------------------------
    public class FaultCodeViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public FaultCategory CategoryEnum { get; set; }
        public string Category => CategoryEnum.ToString();
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    // ----------------------------
    // 7️⃣ RAPOR MODELİ
    // ----------------------------
    public class MaintenanceReportViewModel
    {
        public int Id { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string? FaultName { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Cost { get; set; }
        public bool IsCompleted => EndDate.HasValue;
    }

    // ----------------------------
    // 8️⃣ ENUM: BAKIM TÜRÜ
    // ----------------------------
    public enum MaintenanceType
    {
        [Display(Name = "Periyodik Bakım")]
        Periodic = 1,

        [Display(Name = "Arıza Onarımı")]
        Repair = 2,

        [Display(Name = "Kontrol / Denetim")]
        Inspection = 3,

        [Display(Name = "Diğer")]
        Other = 4
    }
}
