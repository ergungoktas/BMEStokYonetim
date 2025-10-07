using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEStokYonetim.Data.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Alt kategori zorunludur")]
        public int CategoryId { get; set; }   // ✅ ProductSubCategoryId

        [ForeignKey(nameof(CategoryId))]
        public ProductSubCategory Category { get; set; } = default!;   // ✅ Navigation

        [Required(ErrorMessage = "Kod zorunludur"), StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün adı zorunludur"), StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Birim seçilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen geçerli bir birim seçin")]
        public ProductUnit Unit { get; set; } = ProductUnit.None;

        public bool IsActive { get; set; } = true;

        public int CurrentStock { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Min. stok 0'dan büyük olmalı")]
        public int? MinStock { get; set; }

        [StringLength(150)]
        public string? Description { get; set; }

        // Navigation Properties
        public ICollection<RequestItem> RequestItems { get; set; } = [];
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = [];
    }
}
