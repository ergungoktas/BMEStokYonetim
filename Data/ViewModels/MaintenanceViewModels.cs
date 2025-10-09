using System;
using System.ComponentModel.DataAnnotations;

namespace BMEStokYonetim.Data.ViewModels
{
    public class MaintenanceFormModel
    {
        [Required(ErrorMessage = "Varlık seçiniz")]
        [Display(Name = "Varlık")]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Arıza kodu seçiniz")]
        [Display(Name = "Arıza Kodu")]
        public int? FaultCodeId { get; set; }

        [Required(ErrorMessage = "Arıza açıklaması zorunludur")]
        [MaxLength(500, ErrorMessage = "Açıklama 500 karakteri geçemez")]
        [Display(Name = "Arıza Açıklaması")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Planlanan Tarih")]
        public DateTime? PlannedDate { get; set; }
    }

    public class MaintenancePartInputModel
    {
        [Required(ErrorMessage = "Ürün seçiniz")]
        [Display(Name = "Ürün")]
        public int? ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1 veya daha büyük olmalıdır")]
        [Display(Name = "Miktar")]
        public int Quantity { get; set; } = 1;

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Birim maliyet 0 veya daha büyük olmalı")]
        [Display(Name = "Birim Maliyet")]
        public decimal UnitCost { get; set; }
    }

    public class MaintenancePersonnelInputModel
    {
        [Required(ErrorMessage = "Personel adı zorunludur")]
        [MaxLength(150)]
        [Display(Name = "Personel Adı")]
        public string PersonnelName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Rol")]
        public string? Role { get; set; }

        [Range(typeof(decimal), "0.25", "79228162514264337593543950335", ErrorMessage = "Çalışma saati 0.25 ve üzeri olmalı")]
        [Display(Name = "Çalışma Saati")]
        public decimal HoursWorked { get; set; } = 1;

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Saatlik ücret 0 veya üzeri olmalı")]
        [Display(Name = "Saatlik Ücret")]
        public decimal HourlyRate { get; set; }
    }

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

        [Required]
        [MaxLength(100)]
        [Display(Name = "Kategori")]
        public string Category { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
