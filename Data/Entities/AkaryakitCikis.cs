
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMEStokYonetim.Data.Entities
{
    [Table("AkaryakitCikislar")]
    public class AkaryakitCikis
    {
        [Key] public int YakitCikisID { get; set; }
        [Required] public int KaynakIstasyonID { get; set; }
        [ForeignKey("KaynakIstasyonID")] public AkaryakitIstasyon KaynakIstasyon { get; set; } = null!;
        [Required, StringLength(10)] public string HedefTip { get; set; } = string.Empty;
        [Required] public int HedefID { get; set; }
        public int? KM { get; set; }
        public int? CalismaSaati { get; set; }
        [Required] public DateTime Tarih { get; set; }
        [Required] public int MiktarLitre { get; set; }
        [StringLength(500)] public string? Aciklama { get; set; }
    }
}
