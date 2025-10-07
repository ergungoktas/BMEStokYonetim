
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("AkaryakitGirisler")]
    public class AkaryakitGiris
    {
        [Key] public int YakitGirisID { get; set; }
        [Required] public int IstasyonID { get; set; }
        [ForeignKey("IstasyonID")] public AkaryakitIstasyon Istasyon { get; set; } = null!;
        [Required] public DateTime Tarih { get; set; }
        [Required] public int MiktarLitre { get; set; }
        [StringLength(500)] public string? Aciklama { get; set; }
    }
}
