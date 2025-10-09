using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Services.Service
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateMaintenanceAsync(Maintenance maintenance, string userId, CancellationToken cancellationToken = default)
        {
            maintenance.CreatedByUserId = userId;
            if (maintenance.RequestDate == default)
            {
                maintenance.RequestDate = DateTime.UtcNow;
            }

            _ = _context.Maintenances.Add(maintenance);
            await _context.SaveChangesAsync(cancellationToken);
            return maintenance.Id;
        }

        public async Task<List<Maintenance>> GetMaintenanceListAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Maintenances
                .Include(m => m.Asset)
                .Include(m => m.FaultCode)
                .OrderByDescending(m => m.RequestDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Maintenance?> GetMaintenanceAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Maintenances
                .Include(m => m.Parts).ThenInclude(p => p.Product)
                .Include(m => m.Personnels)
                .Include(m => m.Asset)
                .Include(m => m.FaultCode)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task UpdateStatusAsync(int maintenanceId, BakimDurumu status, string userId, CancellationToken cancellationToken = default)
        {
            Maintenance? entity = await _context.Maintenances.FirstOrDefaultAsync(m => m.Id == maintenanceId, cancellationToken);
            if (entity == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(entity.CreatedByUserId))
            {
                entity.CreatedByUserId = userId;
            }

            entity.Status = status;

            if (status == BakimDurumu.MaintenanceInProgress && entity.StartDate is null)
            {
                entity.StartDate = DateTime.UtcNow;
            }

            if (status == BakimDurumu.MaintenanceCompleted)
            {
                entity.EndDate = DateTime.UtcNow;
                if (entity.StartDate is null)
                {
                    entity.StartDate = entity.EndDate;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<MaintenancePart?> AddPartAsync(int maintenanceId, int productId, int quantity, decimal unitCost, CancellationToken cancellationToken = default)
        {
            if (quantity <= 0)
            {
                return null;
            }

            if (unitCost < 0)
            {
                return null;
            }

            Maintenance? maintenance = await _context.Maintenances.FindAsync(new object?[] { maintenanceId }, cancellationToken);
            if (maintenance == null)
            {
                return null;
            }

            bool productExists = await _context.Products.AnyAsync(p => p.Id == productId, cancellationToken);
            if (!productExists)
            {
                return null;
            }

            MaintenancePart part = new()
            {
                MaintenanceId = maintenanceId,
                ProductId = productId,
                Quantity = quantity,
                UnitCost = unitCost
            };

            _ = _context.MaintenanceParts.Add(part);
            await _context.SaveChangesAsync(cancellationToken);

            await RecalculateTotalsAsync(maintenanceId, cancellationToken);
            return part;
        }

        public async Task RemovePartAsync(int partId, CancellationToken cancellationToken = default)
        {
            MaintenancePart? part = await _context.MaintenanceParts.FirstOrDefaultAsync(p => p.Id == partId, cancellationToken);
            if (part == null)
            {
                return;
            }

            int maintenanceId = part.MaintenanceId;
            _ = _context.MaintenanceParts.Remove(part);
            await _context.SaveChangesAsync(cancellationToken);
            await RecalculateTotalsAsync(maintenanceId, cancellationToken);
        }

        public async Task<MaintenancePersonnel?> AddPersonnelAsync(int maintenanceId, string personnelName, decimal hours, decimal rate, string? userId = null, string? role = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(personnelName) || hours <= 0 || rate < 0)
            {
                return null;
            }

            Maintenance? maintenance = await _context.Maintenances.FindAsync(new object?[] { maintenanceId }, cancellationToken);
            if (maintenance == null)
            {
                return null;
            }

            MaintenancePersonnel personnel = new()
            {
                MaintenanceId = maintenanceId,
                PersonnelName = personnelName.Trim(),
                HoursWorked = hours,
                HourlyRate = rate,
                UserId = userId,
                Role = string.IsNullOrWhiteSpace(role) ? null : role.Trim()
            };

            _ = _context.MaintenancePersonnels.Add(personnel);
            await _context.SaveChangesAsync(cancellationToken);

            await RecalculateTotalsAsync(maintenanceId, cancellationToken);
            return personnel;
        }

        public async Task RemovePersonnelAsync(int personnelId, CancellationToken cancellationToken = default)
        {
            MaintenancePersonnel? personnel = await _context.MaintenancePersonnels.FirstOrDefaultAsync(p => p.Id == personnelId, cancellationToken);
            if (personnel == null)
            {
                return;
            }

            int maintenanceId = personnel.MaintenanceId;
            _ = _context.MaintenancePersonnels.Remove(personnel);
            await _context.SaveChangesAsync(cancellationToken);
            await RecalculateTotalsAsync(maintenanceId, cancellationToken);
        }

        public async Task UpdateWorkInfoAsync(int maintenanceId, string? workNotes, DateTime? plannedDate = null, CancellationToken cancellationToken = default)
        {
            Maintenance? entity = await _context.Maintenances.FirstOrDefaultAsync(m => m.Id == maintenanceId, cancellationToken);
            if (entity == null)
            {
                return;
            }

            entity.WorkNotes = string.IsNullOrWhiteSpace(workNotes) ? null : workNotes.Trim();
            entity.PlannedDate = plannedDate;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<FaultCode>> GetFaultCodesAsync(bool onlyActive = true, CancellationToken cancellationToken = default)
        {
            IQueryable<FaultCode> query = _context.FaultCodes.AsNoTracking();
            if (onlyActive)
            {
                query = query.Where(fc => fc.IsActive);
            }

            return await query
                .OrderBy(fc => fc.Category)
                .ThenBy(fc => fc.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<FaultCode> CreateFaultCodeAsync(FaultCode faultCode, CancellationToken cancellationToken = default)
        {
            faultCode.Code = faultCode.Code.Trim();
            faultCode.Name = faultCode.Name.Trim();
            faultCode.Category = faultCode.Category.Trim();
            faultCode.Description = string.IsNullOrWhiteSpace(faultCode.Description) ? null : faultCode.Description.Trim();

            _ = _context.FaultCodes.Add(faultCode);
            await _context.SaveChangesAsync(cancellationToken);
            return faultCode;
        }

        public async Task UpdateFaultCodeStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default)
        {
            FaultCode? faultCode = await _context.FaultCodes.FirstOrDefaultAsync(fc => fc.Id == id, cancellationToken);
            if (faultCode == null)
            {
                return;
            }

            faultCode.IsActive = isActive;
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task RecalculateTotalsAsync(int maintenanceId, CancellationToken cancellationToken = default)
        {
            Maintenance? maintenance = await _context.Maintenances
                .Include(m => m.Parts)
                .Include(m => m.Personnels)
                .FirstOrDefaultAsync(m => m.Id == maintenanceId, cancellationToken);

            if (maintenance == null)
            {
                return;
            }

            decimal laborHours = maintenance.Personnels.Sum(p => p.HoursWorked);
            decimal laborCost = maintenance.Personnels.Sum(p => p.TotalCost);
            decimal partsCost = maintenance.Parts.Sum(p => p.TotalCost);

            maintenance.LaborHours = laborHours;
            maintenance.LaborCost = laborCost;
            maintenance.TotalCost = laborCost + partsCost;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
