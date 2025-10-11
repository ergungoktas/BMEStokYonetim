using System.ComponentModel; // Description niteliğini kullanmak için gerekli

/// <summary>
/// Satın Alma Talep Durumları
/// </summary>
/// 
public enum BakimTuru
{
    [Description("Periyodik Bakım")]
    PeriyodikBakim = 1,

    [Description("Arıza Onarımı")]
    ArizaOnarimi = 2,

    [Description("Önleyici Bakım")]
    OnleyiciBakim = 3,

    [Description("Revizyon")]
    Revizyon = 4,

    [Description("Modifikasyon")]
    Modifikasyon = 5,

    [Description("Acil Onarım")]
    AcilOnarim = 6
}
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
    [Description("Talep Oluşturuldu")]
    TalepOlusturuldu = 0,

    [Description("Yönetici Onayı Bekliyor")]
    YoneticiOnayiBekliyor = 1,

    [Description("Yönetici Tarafından Reddedildi")]
    YoneticiReddi = 2,

    [Description("Bakım Planlandı")]
    BakimPlanlandi = 3,

    [Description("Ekip Atandı")]
    EkipAtandi = 4,

    [Description("Bakım Başladı")]
    BakimBasladi = 5,

    [Description("Parça Bekleniyor")]
    ParcaBekleniyor = 6,

    [Description("Bakım Devam Ediyor")]
    BakimDevamEdiyor = 7,

    [Description("Bakım Tamamlandı")]
    BakimTamamlandi = 8,

    [Description("Test Ediliyor")]
    TestEdiliyor = 9,

    [Description("Teslim Edildi")]
    TeslimEdildi = 10,

    [Description("Yeniden Açıldı")]
    YenidenAcildi = 11,

    [Description("İptal Edildi")]
    IptalEdildi = 12
}
public enum BakimOncelik
{
    [Description("Düşük")]
    Dusuk = 1,

    [Description("Normal")]
    Normal = 2,

    [Description("Yüksek")]
    Yuksek = 3,

    [Description("Acil")]
    Acil = 4,

    [Description("Kritik")]
    Kritik = 5
}
public enum RezervasyonDurumu
{
    [Description("Talep edildi")]
    RequestCreated = 0,
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
    MobileDepot = 2,

    [Description("Diğer")]
    Other = 3
}
public enum FaultCategory
{
    [Description("Motor")]
    Engine = 1,
    [Description("Hidrolik")]
    Hydraulic = 2,
    [Description("Elektrik")]
    Electrical = 3,
    [Description("Şanzıman")]
    Transmission = 4,
    [Description("Fren Sistemi")]
    BrakeSystem = 5,
    [Description("Soğutma Sistemi")]
    CoolingSystem = 6,
    [Description("Elektronik Sistemi")]
    Electronic = 7,
    [Description("Şase/Kasa v.b")]
    Chassis = 8,
    [Description("Diğer")]
    Other = 9
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