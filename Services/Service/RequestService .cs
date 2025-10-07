using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace BMEStokYonetim.Services.Service
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProcessService _processService;

        public RequestService(ApplicationDbContext context, IProcessService processService)
        {
            _context = context;
            _processService = processService;
        }

        // ✅ Yeni talep oluştur (ProcessHistory dahil)
        public async Task<Request> CreateRequestAsync(Request request, string userId)
        {
            // 🔹 1. Talep numarasını oluştur
            request.RequestNumber = await GenerateRequestNumberAsync(request.LocationId ?? 0);
            request.RequestDate = DateTime.Now;

            _ = _context.Requests.Add(request);
            _ = await _context.SaveChangesAsync();

            // 🔹 2. Kalem numaralarını ata
            await AssignRequestItemNumbers(request);

            // 🔹 3. Her kalem için başlangıç durumu (Open)
            foreach (RequestItem item in request.Items)
            {
                item.Status = TalepDurumu.Open;

                await _processService.LogProcessHistoryAsync(
                    "RequestItem",
                    item.Id,
                    TalepDurumu.Open,
                    OnayAsamasi.None,
                    userId
                );
            }

            // 🔹 4. Talebin kendisi için de log kaydı
            await _processService.LogProcessHistoryAsync(
                "Request",
                request.Id,
                TalepDurumu.Open,
                OnayAsamasi.None,
                userId
            );

            _ = await _context.SaveChangesAsync();
            return request;
        }

        // ✅ RequestNumber üretimi (BME-TL-YY-XXX-00001 formatı)
        public async Task<string> GenerateRequestNumberAsync(int locationId)
        {
            string locationName = await _context.Locations
                .Where(l => l.Id == locationId)
                .Select(l => l.Name)
                .FirstOrDefaultAsync() ?? "XXX";

            string locationCode = NormalizeToAscii(locationName)
                .Replace(" ", "")
                .ToUpperInvariant();

            if (locationCode.Length > 3)
            {
                locationCode = locationCode[..3];
            }

            string yearCode = DateTime.Now.ToString("yy");
            string prefix = $"BME-TL-{yearCode}-{locationCode}";

            string? lastNumber = await _context.Requests
                .Where(r => r.RequestNumber.StartsWith(prefix))
                .OrderByDescending(r => r.RequestNumber)
                .Select(r => r.RequestNumber)
                .FirstOrDefaultAsync();

            int next = 1;
            if (!string.IsNullOrEmpty(lastNumber))
            {
                string[] parts = lastNumber.Split('-');
                if (parts.Length >= 5 && int.TryParse(parts[4], out int parsed))
                {
                    next = parsed + 1;
                }
            }

            return $"{prefix}-{next:D5}";
        }

        // ✅ Kalem bazlı numara (ör: BME-TL-YY-XXX-00001-01)
        public Task AssignRequestItemNumbers(Request request)
        {
            int counter = 1;
            foreach (RequestItem item in request.Items)
            {
                item.RequestItemNr = $"{request.RequestNumber}-{counter:D2}";
                counter++;
            }
            return Task.CompletedTask;
        }

        // 🔹 Türkçe karakterleri ASCII'ye çeviren yardımcı fonksiyon
        private static string NormalizeToAscii(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "XXX";
            }

            text = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char c in text)
            {
                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark)
                {
                    _ = sb.Append(c);
                }
            }

            return sb.ToString()
                .Replace("İ", "I").Replace("ı", "i")
                .Replace("Ç", "C").Replace("ç", "c")
                .Replace("Ğ", "G").Replace("ğ", "g")
                .Replace("Ö", "O").Replace("ö", "o")
                .Replace("Ş", "S").Replace("ş", "s")
                .Replace("Ü", "U").Replace("ü", "u");
        }
    }
}
