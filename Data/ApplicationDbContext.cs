using BMEStokYonetim.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BMEStokYonetim.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Request> Requests => Set<Request>();
        public DbSet<RequestItem> RequestItems => Set<RequestItem>();
        public DbSet<AssetExternalRepair> AssetExternalRepairs => Set<AssetExternalRepair>();
        public DbSet<AssetUsageLog> AssetUsageLogs => Set<AssetUsageLog>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
        public DbSet<ProcessHistory> ProcessHistories => Set<ProcessHistory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductSubCategory> ProductSubCategories => Set<ProductSubCategory>();
        public DbSet<ProductMainCategory> ProductMainCategories => Set<ProductMainCategory>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Location> Locations => Set<Location>();

        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<Maintenance> Maintenances => Set<Maintenance>();
        public DbSet<MaintenancePart> MaintenanceParts => Set<MaintenancePart>();
        public DbSet<MaintenancePersonnel> MaintenancePersonnels => Set<MaintenancePersonnel>();

        public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<StockReservation> StockReservations => Set<StockReservation>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region RequestItem Configuration
            _ = builder.Entity<RequestItem>(entity =>
            {
                _ = entity.HasOne(ri => ri.Request)
                      .WithMany(r => r.Items)
                      .HasForeignKey(ri => ri.RequestId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(ri => ri.Product)
                      .WithMany(p => p.RequestItems)
                      .HasForeignKey(ri => ri.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(ri => ri.PurchaseDetail)
                      .WithMany()
                      .HasForeignKey(ri => ri.PurchaseDetailId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(ri => ri.TargetWarehouse)
                      .WithMany()
                      .HasForeignKey(ri => ri.TargetWarehouseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region PurchaseDetail Configuration
            _ = builder.Entity<PurchaseDetail>(entity =>
            {
                _ = entity.Property(pd => pd.UnitPrice).HasPrecision(18, 2);

                _ = entity.HasOne(pd => pd.Purchase)
                      .WithMany(p => p.Details)
                      .HasForeignKey(pd => pd.PurchaseId)
                      .OnDelete(DeleteBehavior.Cascade);

                _ = entity.HasOne(pd => pd.Product)
                      .WithMany(p => p.PurchaseDetails)
                      .HasForeignKey(pd => pd.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(pd => pd.Supplier)
                      .WithMany(s => s.PurchaseDetails)
                      .HasForeignKey(pd => pd.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(pd => pd.RequestItem)
                      .WithMany()
                      .HasForeignKey(pd => pd.RequestItemId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region StockMovement Configuration
            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.Product)
                .WithMany()
                .HasForeignKey(sm => sm.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.SourceLocation)
                .WithMany(l => l.SourceStockMovements)
                .HasForeignKey(sm => sm.SourceLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.TargetLocation)
                .WithMany(l => l.TargetStockMovements)
                .HasForeignKey(sm => sm.TargetLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.SourceWarehouse)
                .WithMany(w => w.SourceStockMovements)
                .HasForeignKey(sm => sm.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.TargetWarehouse)
                .WithMany(w => w.TargetStockMovements)
                .HasForeignKey(sm => sm.TargetWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.Asset)
                .WithMany()
                .HasForeignKey(sm => sm.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.Maintenance)
                .WithMany()
                .HasForeignKey(sm => sm.MaintenanceId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.PurchaseDetail)
                .WithMany(pd => pd.StockMovements)
                .HasForeignKey(sm => sm.PurchaseDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.RequestItem)
                .WithMany()
                .HasForeignKey(sm => sm.RequestItemId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.Entity<StockMovement>()
                .HasOne(sm => sm.User)
                .WithMany()
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Location Configuration
            _ = builder.Entity<Location>(entity =>
            {
                _ = entity.HasMany(l => l.SourceStockMovements)
                      .WithOne(sm => sm.SourceLocation)
                      .HasForeignKey(sm => sm.SourceLocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasMany(l => l.TargetStockMovements)
                      .WithOne(sm => sm.TargetLocation)
                      .HasForeignKey(sm => sm.TargetLocationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Warehouse Configuration
            _ = builder.Entity<Warehouse>(entity =>
            {
                _ = entity.HasIndex(w => w.Code).IsUnique();


            });

            _ = builder.Entity<WarehouseStock>(entity =>
            {
                _ = entity.HasIndex(ws => new { ws.WarehouseId, ws.ProductId }).IsUnique();

                _ = entity.HasOne(ws => ws.Warehouse)
                      .WithMany(w => w.Stocks)
                      .HasForeignKey(ws => ws.WarehouseId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(ws => ws.Product)
                      .WithMany()
                      .HasForeignKey(ws => ws.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region StockReservation Configuration
            _ = builder.Entity<StockReservation>(entity =>
            {
                _ = entity.HasOne(sr => sr.RequestItem)
                      .WithMany(ri => ri.Reservations)
                      .HasForeignKey(sr => sr.RequestItemId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(sr => sr.Product)
                      .WithMany()
                      .HasForeignKey(sr => sr.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                _ = entity.HasOne(sr => sr.Warehouse)
                      .WithMany()
                      .HasForeignKey(sr => sr.WarehouseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Maintenance Configuration
            _ = builder.Entity<Maintenance>(entity =>
            {
                _ = entity.Property(m => m.LaborCost).HasPrecision(18, 2);
                _ = entity.Property(m => m.LaborHours).HasPrecision(18, 2);
                _ = entity.Property(m => m.TotalCost).HasPrecision(18, 2);
            });

            _ = builder.Entity<MaintenancePart>(entity =>
            {
                _ = entity.Property(mp => mp.UnitCost).HasPrecision(18, 2);
                _ = entity.Ignore(mp => mp.TotalCost);
            });

            _ = builder.Entity<MaintenancePersonnel>(entity =>
            {
                _ = entity.Property(mp => mp.HoursWorked).HasPrecision(18, 2);
                _ = entity.Property(mp => mp.HourlyRate).HasPrecision(18, 2);
                _ = entity.Ignore(mp => mp.TotalCost);
            });
            #endregion

            #region Other Configurations
            _ = builder.Entity<Purchase>(entity =>
            {
                _ = entity.HasIndex(p => p.PurchaseNumber).IsUnique();
            });

            _ = builder.Entity<Request>(entity =>
            {
                _ = entity.HasIndex(r => r.RequestNumber).IsUnique();
            });

            _ = builder.Entity<Product>(entity =>
            {
                _ = entity.HasIndex(p => p.Code).IsUnique();
            });

            _ = builder.Entity<Asset>(entity =>
            {
                _ = entity.HasOne(a => a.Category)
                      .WithMany(ac => ac.Assets)
                      .HasForeignKey(a => a.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion
        }
    }
}
