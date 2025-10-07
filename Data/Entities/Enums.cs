using System.ComponentModel; // Description niteliğini kullanmak için gerekli

/// <summary>
/// Satın Alma Talep Durumları
/// </summary>
/// 
public enum CurrencyType
{
    [Description("Türk Lirası (₺)")]
    TRY = 1,
    [Description("Amerikan Doları ($)")]
    USD = 2,
    [Description("Euro (€)")]
    EUR = 3
}
public enum OnayAsamasi
{
    [Description("Bekliyor")]
    None = 0,

    [Description("Birim Onayladı")]
    UnitApproved = 1,

    [Description("Yönetim Onayladı")]
    ManagementApproved = 2,

    [Description("Reddedildi")]
    Rejected = 3
}

public enum ProductUnit
{
    [Description("Seçiniz")] // 0 = default, seçilmemiş
    None = 0,
    [Description("Adet")]
    Adet = 1,

    [Description("Metre")]
    Metre = 2,

    [Description("Kilogram")]
    Kilogram = 3,

    [Description("Litre")]
    Litre = 4,

    [Description("Gram")]
    Gram = 5,

    [Description("Mililitre")]
    Mililitre = 6,

    [Description("Santimetre")]
    Santimetre = 7,

    [Description("Paket")]
    Paket = 8,

    [Description("Kutu")]
    Kutu = 9,

    [Description("Rulo")]
    Rulo = 10,

    [Description("Set")]
    Set = 11,

    [Description("Çift")]
    Çift = 12,

    [Description("Tabaka")]
    Tabaka = 13
}
public enum MovementType
{
    [Description("Stok Girişi")]
    In = 1,

    [Description("Stok Çıkışı")]
    Out = 2,

    [Description("Transfer")]
    Transfer = 3
}
public enum TalepDurumu
{
    [Description("Talep oluşturuldu, onay bekliyor")]
    Open = 1,

    [Description("Talep onaylandı, satınalma bekliyor")]
    Approved = 2,

    [Description("Talep reddedildi")]
    Rejected = 3,

    [Description("Satınalma oluşturuldu, onay bekliyor")]
    PurchasePending = 4,

    [Description("Satınalma onaylandı, teslimat bekliyor")]
    PurchaseApproved = 5,

    [Description("Satınalma reddedildi")]
    PurchaseRejected = 6,

    [Description("Kısmi teslimat yapıldı")]
    PartialDelivery = 7,

    [Description("Talep tamamlandı/kapandı")]
    Closed = 8
}
public enum BakimDurumu
{
    [Description("Bakım planlandı")]
    MaintenancePlanned = 1,

    [Description("Bakım devam ediyor")]
    MaintenanceInProgress = 2,

    [Description("Bakım tamamlandı")]
    MaintenanceCompleted = 3
}
public enum RezervasyonDurumu
{
    [Description("Rezervasyon aktif")]
    ReservationActive = 1,

    [Description("Rezervasyon tamamlandı")]
    ReservationCompleted = 2,

    [Description("Rezervasyon süresi doldu")]
    ReservationExpired = 3,

    [Description("Rezervasyon iptal edildi")]
    ReservationCancelled = 4
}
public enum ReservationType
{
    Automatic = 1,
    Manual = 2
}

public enum WarehouseType
{
    [Description("Ana Depo")]
    MainDepot = 1,

    [Description("Mobil Depo")]
    MobileDepot = 2
}

// NOT: Description niteliğindeki metni okumak isterseniz,
// aşağıdaki gibi bir yardımcı (extension) metot kullanmanız gerekir.

/*
public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
}

// KULLANIM ÖRNEĞİ:
// string aciklama = TalepDurumu.Open.GetDescription(); // "Talep oluşturuldu, onay bekliyor" döner
*/