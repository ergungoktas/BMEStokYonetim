# Önerilen Görevler

## 1. Yazım Hatası Düzeltme
*Dosya:* `Pages/Tanimlar/Tedarikciler.razor`

Tedarikçi listesi sayfasındaki liste göster/gizle butonu "Listeyi GÃ¶ster/Gizle" şeklinde bozuk bir karakter dizisi gösteriyor. UTF-8 karakteri yanlış işlendiğinden kullanıcı arayüzünde "Göster" kelimesi okunamıyor (satır 35). "GÃ¶ster" ifadesini "Göster" olarak düzeltmek, Türkçe karakterlerin doğru görünmesini sağlayacaktır. 【F:Pages/Tanimlar/Tedarikciler.razor†L35-L36】

## 2. Hata Düzeltme
*Dosya:* `Services/Service/StockService.cs`

`ManualReserveAsync` metodu yeni bir `StockReservation` oluşturuyor ancak ilgili deponun stok kayıtlarında ayırma yapmıyor. Sonuç olarak rezervasyon kaydı oluşturulsa bile `ReservedQuantity` değeri artmadığı için stok raporları ve kullanılabilir miktar hesapları yanlış kalıyor (satırlar 220-233). Rezervasyon oluşturulurken `_warehouseService.ReserveStockAsync` çağrısı eklenmeli veya stok kaydının `ReservedQuantity` alanı güncellenmelidir. 【F:Services/Service/StockService.cs†L218-L234】

## 3. Dokümantasyon/Kommentar Tutarsızlığı
*Dosya:* `Services/Service/FuelService.cs`

`CreateOrUpdateStationAsync` metodunun içindeki yorum satırı `Warehouse` sınıfında `Description/TypeId/ParentWarehouseId` alanları olmayabileceğini söylüyor (satırlar 49-50). Ancak güncel `Warehouse` varlığı bu alanlara sahip değil; bunun yerine `Type` (enum) alanı bulunuyor. Yorum, artık var olmayan alanlardan bahsederek geliştiricileri yanıltıyor. Yorum, gerçek alan adlarını yansıtacak şekilde güncellenmelidir. 【F:Services/Service/FuelService.cs†L45-L51】【F:Data/Entities/Warehouse.cs†L9-L20】

## 4. Test İyileştirmesi
*Dosya:* `Services/Service/PurchaseService.cs`

`GeneratePurchaseNumberAsync` metodu satın alma numarasını `BME-PO-YY-LOC-XXXXX` formatında üretmekten sorumlu (satırlar 71-116). Bu kritik iş kuralı için herhangi bir birim testi bulunmuyor. Farklı lokasyon adları, Türkçe karakterler ve art arda çağrılar için numaralandırmanın doğru ilerlediğini doğrulayan bir birim testi eklenmesi, olası regresyonları önleyecektir. 【F:Services/Service/PurchaseService.cs†L71-L121】
