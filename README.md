# BMEStokYonetim

## Testleri Çalıştırma

Projede yer alan testleri çalıştırmak için sisteminizde .NET SDK yüklü olmalıdır. Ortamda `dotnet` komutu yoksa aşağıdaki adımları izleyerek SDK'yı kurabilirsiniz.

### Linux (Ubuntu/Debian)

1. Microsoft paket deposunu sisteme ekleyin:
   ```bash
   wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   rm packages-microsoft-prod.deb
   ```
2. Paket listesini güncelleyin ve .NET SDK'yı kurun:
   ```bash
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-8.0
   ```

### Windows

1. [Microsoft'un .NET indirme sayfasına](https://dotnet.microsoft.com/en-us/download) gidin.
2. Kullandığınız işletim sistemi için uygun olan SDK kurulum paketini indirin ve çalıştırın.

### Kurulumu Doğrulama ve Testleri Çalıştırma

Kurulum tamamlandıktan sonra komut satırında aşağıdaki komut ile sürümü kontrol edebilirsiniz:

```bash
dotnet --version
```

Ardından proje dizininde testleri çalıştırmak için:

```bash
dotnet test
```
