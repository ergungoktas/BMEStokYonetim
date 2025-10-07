
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("Locations")]
    public class Location
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public bool IsMainDepot { get; set; } = false; // Yeni alan
        [StringLength(255)]
        public string? Description { get; set; }

        // Lokasyondaki stok hareketleri
        public virtual ICollection<StockMovement> SourceStockMovements { get; set; } = [];
        public virtual ICollection<StockMovement> TargetStockMovements { get; set; } = [];
        public ICollection<Warehouse> Warehouses { get; set; } = [];
    }
}
