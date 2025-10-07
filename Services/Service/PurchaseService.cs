using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace BMEStokYonetim.Services.Service
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IProcessService _processService;

        public PurchaseService(IDbContextFactory<ApplicationDbContext> contextFactory, IProcessService processService)
        {
            _contextFactory = contextFactory;
            _processService = processService;
        }

        // 🔹 Yeni satınalma oluştur
        public async Task<Purchase> CreatePurchaseAsync(Purchase purchase, List<PurchaseDetail> details, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

            purchase.PurchaseDate = DateTime.Now;
            purchase.CreatedByUserId = userId;

            _ = await context.Purchases.AddAsync(purchase);
            _ = await context.SaveChangesAsync();

            // 🔹 Kalemleri oluştur
            foreach (PurchaseDetail d in details)
            {
                d.PurchaseId = purchase.Id;
                d.Status = TalepDurumu.PurchasePending;  // kalem bazlı durum
                d.ApprovalStage = OnayAsamasi.None;
                _ = await context.PurchaseDetails.AddAsync(d);
            }

            _ = await context.SaveChangesAsync();

            // 🔹 ProcessHistory: Her PurchaseDetail için log kaydı
            foreach (PurchaseDetail d in details)
            {
                await _processService.LogProcessHistoryAsync(
                    "PurchaseDetail",
                    d.Id,
                    TalepDurumu.PurchasePending,
                    OnayAsamasi.None,
                    userId);
            }

            // 🔹 İlgili RequestItem'ları güncelle
            List<int> reqItemIds = details
                .Where(x => x.RequestItemId.HasValue)
                .Select(x => x.RequestItemId!.Value)
                .Distinct()
                .ToList();

            if (reqItemIds.Any())
            {
                List<RequestItem> reqItems = await context.RequestItems
                    .Where(ri => reqItemIds.Contains(ri.Id))
                    .ToListAsync();

                foreach (RequestItem ri in reqItems)
                {
                    ri.Status = TalepDurumu.PurchasePending;
                    ri.ApprovalStage = OnayAsamasi.None;

                    await _processService.LogProcessHistoryAsync(
                        "RequestItem",
                        ri.Id,
                        TalepDurumu.PurchasePending,
                        OnayAsamasi.None,
                        userId);
                }
            }

            _ = await context.SaveChangesAsync();

            return purchase;
        }

        // 🔹 Üst amir onayı (birim onayı)
        public async Task ApproveByUnitAsync(int purchaseDetailId, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
            PurchaseDetail? detail = await context.PurchaseDetails.FindAsync(purchaseDetailId);

            if (detail == null)
            {
                return;
            }

            detail.ApprovalStage = OnayAsamasi.UnitApproved;
            _ = await context.SaveChangesAsync();

            await _processService.LogProcessHistoryAsync("PurchaseDetail", detail.Id, TalepDurumu.PurchasePending, OnayAsamasi.UnitApproved, userId);
        }

        // 🔹 Müdür onayı
        public async Task ApproveByManagementAsync(int purchaseDetailId, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

            PurchaseDetail? detail = await context.PurchaseDetails
                .Include(d => d.RequestItem)
                .FirstOrDefaultAsync(d => d.Id == purchaseDetailId);

            if (detail == null)
            {
                return;
            }

            detail.Status = TalepDurumu.PurchaseApproved;
            detail.ApprovalStage = OnayAsamasi.ManagementApproved;

            if (detail.RequestItem != null)
            {
                detail.RequestItem.Status = TalepDurumu.PurchaseApproved;

                await _processService.LogProcessHistoryAsync(
                    "RequestItem",
                    detail.RequestItem.Id,
                    TalepDurumu.PurchaseApproved,
                    OnayAsamasi.ManagementApproved,
                    userId);
            }

            _ = await context.SaveChangesAsync();

            await _processService.LogProcessHistoryAsync(
                "PurchaseDetail",
                detail.Id,
                TalepDurumu.PurchaseApproved,
                OnayAsamasi.ManagementApproved,
                userId);
        }

        // 🔹 Reddetme
        public async Task RejectAsync(int purchaseDetailId, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
            PurchaseDetail? detail = await context.PurchaseDetails.FindAsync(purchaseDetailId);

            if (detail == null)
            {
                return;
            }

            detail.Status = TalepDurumu.PurchaseRejected;
            _ = await context.SaveChangesAsync();

            await _processService.LogProcessHistoryAsync("PurchaseDetail", detail.Id, TalepDurumu.PurchaseRejected, OnayAsamasi.Rejected, userId);
        }

        // ✅ Satınalma numarası (BME-PO-YY-XXX-00001 formatı)
        public async Task<string> GeneratePurchaseNumberAsync(int locationId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

            string locationName = await context.Locations
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
            string prefix = $"BME-PO-{yearCode}-{locationCode}";

            string? lastNumber = await context.Purchases
                .Where(p => p.PurchaseNumber.StartsWith(prefix))
                .OrderByDescending(p => p.PurchaseNumber)
                .Select(p => p.PurchaseNumber)
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

        // 🔤 Türkçe karakter temizleme
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
