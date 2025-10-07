using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("MaintenancePersonnel")]
    public class MaintenancePersonnel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaintenanceId { get; set; }

        [ForeignKey("MaintenanceId")]
        public virtual Maintenance Maintenance { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        // Personelin rolü (ör. Usta, Yardımcı, Elektrikçi vb.)
        [Required, StringLength(100)]
        public string Role { get; set; } = string.Empty;

        // Kullanılacak isim gösterimi (UI için)
        [StringLength(150)]
        public string PersonnelName { get; set; } = string.Empty;

        // Çalıştığı saat (bakım/onarımda)
        public decimal HoursWorked { get; set; }

        // Saatlik ücret
        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        // Toplam maliyet
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost => HoursWorked * HourlyRate;
    }
}
