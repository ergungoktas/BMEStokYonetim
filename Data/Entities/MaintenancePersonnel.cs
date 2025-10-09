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

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        // Personelin rolü (ör. Usta, Yardımcı, Elektrikçi vb.)
        [StringLength(100)]
        public string? Role { get; set; }

        // Kullanılacak isim gösterimi (UI için)
        [Required, StringLength(150)]
        public string PersonnelName { get; set; } = string.Empty;

        // Çalıştığı saat (bakım/onarımda)
        public decimal HoursWorked { get; set; }

        // Saatlik ücret
        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        // Toplam maliyet
        [NotMapped]
        public decimal TotalCost => HoursWorked * HourlyRate;
    }
}
