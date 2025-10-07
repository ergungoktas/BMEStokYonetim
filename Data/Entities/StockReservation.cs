using BMEStokYonetim.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    public class StockReservation
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public int? RequestItemId { get; set; }
        public RequestItem? RequestItem { get; set; }

        public int ReservedQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public ReservationType Type { get; set; } = ReservationType.Automatic;

        public RezervasyonDurumu Status { get; set; } = RezervasyonDurumu.ReservationActive;

        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ReleasedAt { get; set; }

        [NotMapped]
        public string StatusText => Status.GetDescription();
    }
}
