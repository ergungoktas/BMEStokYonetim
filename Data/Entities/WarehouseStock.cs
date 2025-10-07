using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    public class WarehouseStock
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; } = 0;

        public int MinStockLevel { get; set; } = 0;
        public DateTime LastUpdated { get; set; }

        // ✅ Hesaplanan property (DB’de kolon tutmaya gerek yok)
        [NotMapped]
        public int AvailableQuantity => Quantity - ReservedQuantity;
    }
}
