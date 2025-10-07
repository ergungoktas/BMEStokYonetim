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

        public async Task SetRequestItemStatusAsync(int requestItemId,
                                                    TalepDurumu newStatus,
                                                    OnayAsamasi approvalStage,
                                                    string? userId,
                                                    bool log = true)
        {
            string resolvedUserId = ResolveUserId(userId);

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
                        AddProcessHistory(context, "RequestItem", requestItemId, requestItem.Status, requestItem.ApprovalStage, resolvedUserId);
                    }

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }

                requestItem.Status = newStatus;
                requestItem.ApprovalStage = approvalStage;

                if (log)
                {
                    AddProcessHistory(context, "RequestItem", requestItemId, newStatus, approvalStage, resolvedUserId);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task LogProcessHistoryAsync(string entityType,
                                                 int entityId,
                                                 TalepDurumu status,
                                                 OnayAsamasi stage,
                                                 string? userId)
        {
            await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
            string resolvedUserId = ResolveUserId(userId);

<<<<<<< ours
            AddProcessHistory(context, entityType, entityId, status, stage, resolvedUserId);
            await context.SaveChangesAsync();
=======
            ProcessHistory history = new()
            {
                EntityType = entityType,
                EntityId = entityId,
                Status = status,
                ApprovalStage = stage,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _ = context.ProcessHistories.Add(history);
            _ = await context.SaveChangesAsync();
>>>>>>> theirs
        }

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

        public async Task ApproveByUnitAsync(int requestItemId, string? userId)
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

        public async Task ApproveByManagementAsync(int requestItemId, string? userId)
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

        public async Task RejectAsync(int requestItemId, string? userId)
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

        public async Task LogAsync(string entityType, int entityId, TalepDurumu status, string? userId)
        {
            await LogProcessHistoryAsync(entityType, entityId, status, OnayAsamasi.None, userId);
        }

        private static void AddProcessHistory(ApplicationDbContext context,
                                              string entityType,
                                              int entityId,
                                              TalepDurumu status,
                                              OnayAsamasi stage,
                                              string userId)
        {
            ProcessHistory history = new()
            {
                EntityType = entityType,
                EntityId = entityId,
                Status = status,
                ApprovalStage = stage,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            context.ProcessHistories.Add(history);
        }

        private string ResolveUserId(string? userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return userId;
            }

            string? resolved = _userContextService.GetUserId();
            if (string.IsNullOrWhiteSpace(resolved))
            {
                throw new InvalidOperationException("Kullanıcı bilgisi alınamadı.");
            }

            return resolved;
        }
    }
}
