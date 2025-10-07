
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("Suppliers")]
    public class Supplier
    {
        [Key] public int Id { get; set; }
        [Required, StringLength(100)] public string CompanyName { get; set; } = string.Empty;
        [StringLength(255)] public string? ContactAddress { get; set; }
        [Required, StringLength(20)] public string Phone { get; set; } = string.Empty;
        [StringLength(100)] public string? Email { get; set; }
        [StringLength(100)] public string? ContactPerson { get; set; }
        [StringLength(50)] public string? TaxNumber { get; set; }
        [StringLength(100)] public string? TaxOffice { get; set; }
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}
