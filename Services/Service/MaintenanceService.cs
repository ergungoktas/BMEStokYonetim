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

        public async Task<int> CreateMaintenanceAsync(Maintenance maintenance, string userId)
        {
            maintenance.CreatedByUserId = userId;
            _ = _context.Maintenances.Add(maintenance);
            _ = await _context.SaveChangesAsync();
            return maintenance.Id;
        }

        // ---- STATUS GÜNCELLEME ----
        public async Task UpdateStatusAsync(int maintenanceId, BakimDurumu status, string userId)
        {
            Maintenance? entity = await _context.Maintenances.FindAsync(maintenanceId);
            if (entity == null)
            {
                return;
            }

            entity.Status = status;

            if (status == BakimDurumu.MaintenanceInProgress)
            {
                entity.StartDate = DateTime.UtcNow;
            }

            if (status == BakimDurumu.MaintenanceCompleted)
            {
                entity.EndDate = DateTime.UtcNow;
            }

            _ = await _context.SaveChangesAsync();
        }

        // ---- PARÇA EKLEME ----
        public async Task AddPartAsync(int maintenanceId, int productId, int quantity, decimal unitCost, string userId)
        {
            MaintenancePart part = new()
            {
                MaintenanceId = maintenanceId,
                ProductId = productId,
                Quantity = quantity,
                UnitCost = unitCost
            };
            _ = _context.MaintenanceParts.Add(part);

            Product? product = await _context.Products.FindAsync(productId);

            StockMovement sm = new()
            {
                ProductId = productId,
                Quantity = quantity,
                Unit = product?.Unit ?? ProductUnit.Adet, // ✅ Enum kullanımı
                MovementType = MovementType.Out,
                MovementDate = DateTime.UtcNow,
                Description = $"Bakım için stok çıkışı (BakımId={maintenanceId})",
                UserId = userId,
                MaintenanceId = maintenanceId,
                SourceWarehouseId = 1 // Varsayılan depo
            };
            _ = _context.StockMovements.Add(sm);

            _ = await _context.SaveChangesAsync();
        }

        // ---- PERSONEL EKLEME ----
        public async Task AddPersonnelAsync(int maintenanceId, string name, decimal hours, decimal rate)
        {
            MaintenancePersonnel pers = new()
            {
                MaintenanceId = maintenanceId,
                PersonnelName = name,
                HoursWorked = hours,
                HourlyRate = rate
            };

            _ = _context.MaintenancePersonnels.Add(pers);
            _ = await _context.SaveChangesAsync();
        }

        // ---- DETAY GETİRME ----
        public async Task<Maintenance?> GetMaintenanceAsync(int id)
        {
            return await _context.Maintenances
                .Include(m => m.Parts).ThenInclude(p => p.Product)
                .Include(m => m.Personnels)
                .Include(m => m.Asset)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}