
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("AkaryakitIstasyonlar")]
    public class AkaryakitIstasyon
    {
        [Key] public int IstasyonID { get; set; }
        [Required, StringLength(100)] public string Ad { get; set; } = string.Empty;
        [Required, StringLength(20)] public string Tip { get; set; } = string.Empty;
    }
}
