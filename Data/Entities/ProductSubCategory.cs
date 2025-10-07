
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("ProductSubCategories")]
    public class ProductSubCategory
    {
        [Key] public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        [Required] public int ProductMainCategoryId { get; set; }
        public ProductMainCategory ProductMainCategory { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
