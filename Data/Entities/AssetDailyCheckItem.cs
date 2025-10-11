using BMEStokYonetim.Data.Entities;

public class AssetDailyCheckItem
{
    public int Id { get; set; }
    public string Parameter { get; set; } = string.Empty;

    // ❌ eski: public bool? IsOk { get; set; }
    public bool IsOk { get; set; }  // ✅ artık nullable değil
    public string? Notes { get; set; }

    public int DailyCheckId { get; set; }
    public AssetDailyCheck DailyCheck { get; set; } = default!;
}
