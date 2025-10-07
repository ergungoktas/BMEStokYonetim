
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("ProductMainCategories")]
    public class ProductMainCategory
    {
        [Key] public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(255)] public string? Description { get; set; }
        public ICollection<ProductSubCategory> ProductSubCategories { get; set; } = [];

    }
}
