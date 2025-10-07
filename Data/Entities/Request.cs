using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("Requests")]
    public class Request
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(50)] public string RequestNumber { get; set; } = string.Empty;

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Description { get; set; }

        // 🔄 RequestingDepartment yerine Warehouse
        [Required]
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        // Lokasyon
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        // Talep eden kullanıcı
        [Required]
        public string RequestedByUserId { get; set; } = string.Empty;
        public ApplicationUser? RequestedByUser { get; set; }

        // Kalemler
        public ICollection<RequestItem> Items { get; set; } = [];
    }
}
