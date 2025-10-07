using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BMEStokYonetim.Services.Service
{
    public class ProcessService : IProcessService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IUserContextService _userContextService;

        public ProcessService(IDbContextFactory<ApplicationDbContext> contextFactory, IUserContextService userContextService)
        {
            _contextFactory = contextFactory;
            _userContextService = userContextService;
        }

        // ✅ Talep statüsünü güncelle ve süreci logla
        public async Task SetRequestItemStatusAsync(
            int requestItemId,
            TalepDurumu newStatus,
            OnayAsamasi approvalStage,
            string userId,
            bool log = true)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
            await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

            try
            {
                RequestItem? requestItem = await context.RequestItems.FirstOrDefaultAsync(ri => ri.Id == requestItemId);
                if (requestItem == null)
                {
                    throw new InvalidOperationException($"RequestItem bulunamadı. Id={requestItemId}");
                }

                if (requestItem.ApprovalStage == OnayAsamasi.ManagementApproved)
                {
                    if (log)
                    {
                        await LogProcessHistoryAsync("RequestItem", requestItemId, requestItem.Status, requestItem.ApprovalStage, userId);
                    }
                    return;
                }

                requestItem.Status = newStatus;
                requestItem.ApprovalStage = approvalStage;

                _ = await context.SaveChangesAsync();

                if (log)
                {
                    await LogProcessHistoryAsync("RequestItem", requestItemId, newStatus, approvalStage, userId);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ✅ Süreç geçmişi kaydı
        public async Task LogProcessHistoryAsync(string entityType, int entityId,
                                                TalepDurumu status, OnayAsamasi stage, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

            ProcessHistory history = new()
            {
                EntityType = entityType,
                EntityId = entityId,
                Status = status,
                ApprovalStage = stage,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _ = context.ProcessHistories.Add(history);
            _ = await context.SaveChangesAsync();
        }

        // ✅ Belirli varlığın geçmiş kayıtlarını getir
        public async Task<List<ProcessHistory>> GetProcessHistoryAsync(string entityType, int entityId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

            return await context.ProcessHistories
                .AsNoTracking()
                .Include(ph => ph.User)
                .Where(ph => ph.EntityType == entityType && ph.EntityId == entityId)
                .OrderByDescending(ph => ph.CreatedAt)
                .ToListAsync();
        }

        // ✅ Üst amir onayı (birim onayı)
        public async Task ApproveByUnitAsync(int requestItemId, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
            RequestItem? item = await context.RequestItems.FirstOrDefaultAsync(i => i.Id == requestItemId);
            if (item == null)
            {
                throw new InvalidOperationException("RequestItem bulunamadı.");
            }

            if (item.ApprovalStage is OnayAsamasi.UnitApproved or OnayAsamasi.ManagementApproved)
            {
                return;
            }

            await SetRequestItemStatusAsync(requestItemId, TalepDurumu.Open, OnayAsamasi.UnitApproved, userId);
        }

        // ✅ Müdür onayı
        public async Task ApproveByManagementAsync(int requestItemId, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
            RequestItem? item = await context.RequestItems.FirstOrDefaultAsync(i => i.Id == requestItemId);
            if (item == null)
            {
                throw new InvalidOperationException("RequestItem bulunamadı.");
            }

            if (item.ApprovalStage == OnayAsamasi.ManagementApproved)
            {
                return;
            }

            await SetRequestItemStatusAsync(requestItemId, TalepDurumu.Approved, OnayAsamasi.ManagementApproved, userId);
        }

        // ✅ Reddetme
        public async Task RejectAsync(int requestItemId, string userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
            RequestItem? item = await context.RequestItems.FirstOrDefaultAsync(i => i.Id == requestItemId);
            if (item == null)
            {
                throw new InvalidOperationException("RequestItem bulunamadı.");
            }

            if (item.Status == TalepDurumu.Rejected)
            {
                return;
            }

            await SetRequestItemStatusAsync(requestItemId, TalepDurumu.Rejected, OnayAsamasi.Rejected, userId);
        }

        // ✅ Basit log (OnayAsamasi.None)
        public async Task LogAsync(string entityType, int entityId, TalepDurumu status, string userId)
        {
            await LogProcessHistoryAsync(entityType, entityId, status, OnayAsamasi.None, userId);
        }
    }
}
