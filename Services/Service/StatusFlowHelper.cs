using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class StatusFlowHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IProcessService _processService;

        public StatusFlowHelper(ApplicationDbContext context, IProcessService processService)
        {
            _context = context;
            _processService = processService;
        }

        // ✅ Talep oluşturulduğunda (Open)
        public async Task SetRequestCreatedAsync(Request request, string userId)
        {
            foreach (RequestItem item in request.Items)
            {
                item.Status = TalepDurumu.Open;
                await _processService.LogProcessHistoryAsync("RequestItem", item.Id, TalepDurumu.Open, OnayAsamasi.None, userId);
            }
            _ = await _context.SaveChangesAsync();
        }

        // ✅ Üst amir onayı (Approved)
        public async Task SetRequestApprovedAsync(int requestItemId, string userId)
        {
            RequestItem? item = await _context.RequestItems.FindAsync(requestItemId);
            if (item == null)
            {
                return;
            }

            item.Status = TalepDurumu.Approved;
            await _processService.LogProcessHistoryAsync("RequestItem", item.Id, TalepDurumu.Approved, OnayAsamasi.ManagementApproved, userId);
            _ = await _context.SaveChangesAsync();
        }

        // ✅ Satınalma oluşturulunca (PurchasePending)
        public async Task SetPurchaseCreatedAsync(Purchase purchase, List<PurchaseDetail> details, string userId)
        {
            foreach (PurchaseDetail d in details)
            {
                d.Status = TalepDurumu.PurchasePending;
                await _processService.LogProcessHistoryAsync("PurchaseDetail", d.Id, TalepDurumu.PurchasePending, OnayAsamasi.None, userId);

                if (d.RequestItemId.HasValue)
                {
                    RequestItem? req = await _context.RequestItems.FindAsync(d.RequestItemId.Value);
                    if (req != null)
                    {
                        req.Status = TalepDurumu.PurchasePending;
                        await _processService.LogProcessHistoryAsync("RequestItem", req.Id, TalepDurumu.PurchasePending, OnayAsamasi.None, userId);
                    }
                }
            }

            await _processService.LogProcessHistoryAsync("Purchase", purchase.Id, TalepDurumu.PurchasePending, OnayAsamasi.None, userId);
            _ = await _context.SaveChangesAsync();
        }

        // ✅ Satınalma yönetim onayı (PurchaseApproved)
        public async Task SetPurchaseApprovedAsync(int purchaseDetailId, string userId)
        {
            PurchaseDetail? detail = await _context.PurchaseDetails
                .Include(p => p.RequestItem)
                .FirstOrDefaultAsync(p => p.Id == purchaseDetailId);

            if (detail == null)
            {
                return;
            }

            detail.Status = TalepDurumu.PurchaseApproved;
            detail.ApprovalStage = OnayAsamasi.ManagementApproved;

            await _processService.LogProcessHistoryAsync("PurchaseDetail", detail.Id, TalepDurumu.PurchaseApproved, OnayAsamasi.ManagementApproved, userId);

            if (detail.RequestItem != null)
            {
                detail.RequestItem.Status = TalepDurumu.PurchaseApproved;
                await _processService.LogProcessHistoryAsync("RequestItem", detail.RequestItem.Id, TalepDurumu.PurchaseApproved, OnayAsamasi.ManagementApproved, userId);
            }

            _ = await _context.SaveChangesAsync();
        }

        // ✅ Satınalma reddedilirse (PurchaseRejected)
        public async Task SetPurchaseRejectedAsync(int purchaseDetailId, string userId)
        {
            PurchaseDetail? detail = await _context.PurchaseDetails
                .Include(p => p.RequestItem)
                .FirstOrDefaultAsync(p => p.Id == purchaseDetailId);

            if (detail == null)
            {
                return;
            }

            detail.Status = TalepDurumu.PurchaseRejected;
            await _processService.LogProcessHistoryAsync("PurchaseDetail", detail.Id, TalepDurumu.PurchaseRejected, OnayAsamasi.Rejected, userId);

            if (detail.RequestItem != null)
            {
                detail.RequestItem.Status = TalepDurumu.PurchaseRejected;
                await _processService.LogProcessHistoryAsync("RequestItem", detail.RequestItem.Id, TalepDurumu.PurchaseRejected, OnayAsamasi.Rejected, userId);
            }

            _ = await _context.SaveChangesAsync();
        }

        // ✅ Stok girişi sonrası durumu hesapla (PartialDelivery / Closed)
        public async Task SetStockEntryStatusAsync(int purchaseDetailId, string userId)
        {
            PurchaseDetail? detail = await _context.PurchaseDetails
                .Include(d => d.RequestItem)
                .Include(d => d.Product)
                .FirstOrDefaultAsync(d => d.Id == purchaseDetailId);

            if (detail == null)
            {
                return;
            }

            int totalReceived = await _context.StockMovements
                .Where(s => s.PurchaseDetailId == purchaseDetailId && s.MovementType == MovementType.In)
                .SumAsync(s => s.Quantity);

            if (totalReceived < detail.Quantity)
            {
                detail.Status = TalepDurumu.PartialDelivery;
                await _processService.LogProcessHistoryAsync("PurchaseDetail", detail.Id, TalepDurumu.PartialDelivery, OnayAsamasi.None, userId);

                if (detail.RequestItem != null)
                {
                    detail.RequestItem.Status = TalepDurumu.PartialDelivery;
                    await _processService.LogProcessHistoryAsync("RequestItem", detail.RequestItem.Id, TalepDurumu.PartialDelivery, OnayAsamasi.None, userId);
                }
            }
            else
            {
                detail.Status = TalepDurumu.Closed;
                await _processService.LogProcessHistoryAsync("PurchaseDetail", detail.Id, TalepDurumu.Closed, OnayAsamasi.None, userId);

                if (detail.RequestItem != null)
                {
                    detail.RequestItem.Status = TalepDurumu.Closed;
                    await _processService.LogProcessHistoryAsync("RequestItem", detail.RequestItem.Id, TalepDurumu.Closed, OnayAsamasi.None, userId);
                }
            }

            _ = await _context.SaveChangesAsync();
        }
    }
}
