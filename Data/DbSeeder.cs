using BMEStokYonetim.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BMEStokYonetim.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, ILogger? logger = null, CancellationToken cancellationToken = default)
        {
            using IServiceScope scope = serviceProvider.CreateScope();

            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(cancellationToken);

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string adminUserId = await EnsureIdentityAsync(roleManager, userManager, logger, cancellationToken);

            await SeedLocationsAsync(context, logger, cancellationToken);
            await SeedWarehousesAsync(context, logger, cancellationToken);
            await SeedProductHierarchyAsync(context, logger, cancellationToken);
            await SeedSuppliersAsync(context, logger, cancellationToken);
            await SeedProductsAsync(context, logger, cancellationToken);
            await SeedWarehouseStocksAsync(context, logger, cancellationToken);
            await SeedRequestsAndPurchasesAsync(context, adminUserId, logger, cancellationToken);
            await SeedStockMovementsAsync(context, adminUserId, logger, cancellationToken);
            await SeedReservationsAsync(context, adminUserId, logger, cancellationToken);
            await SeedFuelDataAsync(context, logger, cancellationToken);
        }

        private static async Task<string> EnsureIdentityAsync(RoleManager<IdentityRole> roleManager,
                                                              UserManager<ApplicationUser> userManager,
                                                              ILogger? logger,
                                                              CancellationToken cancellationToken)
        {
            string[] roles = ["Admin", "DepoSorumlusu", "Satinalma", "Bakim"];
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        throw new InvalidOperationException($"Rol oluşturulamadı: {role}");
                    logger?.LogInformation("✅ Rol oluşturuldu: {Role}", role);
                }
            }

            const string adminEmail = "admin@bme.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Sistem Yöneticisi",
                    EmailConfirmed = true,
                    PhoneNumber = "+905551112233"
                };
                var createResult = await userManager.CreateAsync(admin, "Admin123!");
                if (!createResult.Succeeded)
                    throw new InvalidOperationException("Admin kullanıcı oluşturulamadı.");
                logger?.LogInformation("✅ Yönetici kullanıcısı oluşturuldu: {Email}", adminEmail);
            }

            foreach (string role in roles)
            {
                if (!await userManager.IsInRoleAsync(admin, role))
                    await userManager.AddToRoleAsync(admin, role);
            }

            return admin.Id;
        }

        private static async Task SeedLocationsAsync(ApplicationDbContext context, ILogger? logger, CancellationToken ct)
        {
            if (await context.Locations.AnyAsync(ct)) return;
            List<Location> list =
            [
                new() { Name = "Merkez Tesis", Description = "Ana operasyon merkezi", IsMainDepot = true },
                new() { Name = "Saha 1", Description = "Şantiye alanı 1" },
                new() { Name = "Saha 2", Description = "Şantiye alanı 2" }
            ];
            await context.Locations.AddRangeAsync(list, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Lokasyonlar eklendi ({Count}).", list.Count);
        }

        private static async Task SeedWarehousesAsync(ApplicationDbContext context, ILogger? logger, CancellationToken ct)
        {
            if (await context.Warehouses.AnyAsync(ct)) return;
            var merkez = await context.Locations.FirstAsync(l => l.Name == "Merkez Tesis", ct);
            var saha1 = await context.Locations.FirstAsync(l => l.Name == "Saha 1", ct);
            List<Warehouse> list =
            [
                new() { Code = "DEP-001", Name = "Merkez Ana Depo", Type = WarehouseType.MainDepot, LocationId = merkez.Id, IsActive = true },
                new() { Code = "DEP-002", Name = "Saha 1 Mobil Depo", Type = WarehouseType.MobileDepot, LocationId = saha1.Id, IsActive = true }
            ];
            await context.Warehouses.AddRangeAsync(list, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Depolar eklendi ({Count}).", list.Count);
        }

        private static async Task SeedProductHierarchyAsync(ApplicationDbContext context, ILogger? logger, CancellationToken ct)
        {
            if (await context.ProductMainCategories.AnyAsync(ct)) return;

            ProductMainCategory consumable = new() { Name = "Sarf Malzemeleri", Description = "Genel sarf malzemeleri" };
            ProductMainCategory mechanical = new() { Name = "Mekanik", Description = "Mekanik ekipman ve parçalar" };
            ProductMainCategory fuel = new() { Name = "Yağ ve Akaryakıt", Description = "Akaryakıt, yağ ve sıvılar" };

            await context.ProductMainCategories.AddRangeAsync([consumable, mechanical, fuel], ct);
            await context.SaveChangesAsync(ct);

            List<ProductSubCategory> subs =
            [
                new() { Name = "Elektrik Sarf", ProductMainCategoryId = consumable.Id },
                new() { Name = "Bağlantı Elemanları", ProductMainCategoryId = mechanical.Id },
                new() { Name = "Motor Yağları", ProductMainCategoryId = fuel.Id }
            ];
            await context.ProductSubCategories.AddRangeAsync(subs, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Ürün hiyerarşisi eklendi.");
        }

        private static async Task SeedSuppliersAsync(ApplicationDbContext context, ILogger? logger, CancellationToken ct)
        {
            if (await context.Suppliers.AnyAsync(ct)) return;
            List<Supplier> list =
            [
                new() { CompanyName = "Enerji Market", Phone = "+903122224455", Email = "info@enerjimarket.com", ContactPerson = "Ayşe Demir" },
                new() { CompanyName = "Mega Teknik", Phone = "+903124445566", Email = "satis@megateknik.com", ContactPerson = "Mehmet Kaya" },
                new() { CompanyName = "Yağ Sanayi", Phone = "+903126667788", Email = "destek@yagsanayi.com", ContactPerson = "Selin Aydın" }
            ];
            await context.Suppliers.AddRangeAsync(list, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Tedarikçiler eklendi.");
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context, ILogger? logger, CancellationToken ct)
        {
            if (await context.Products.AnyAsync(ct)) return;
            var elektrik = await context.ProductSubCategories.FirstAsync(sc => sc.Name == "Elektrik Sarf", ct);
            var baglanti = await context.ProductSubCategories.FirstAsync(sc => sc.Name == "Bağlantı Elemanları", ct);
            var yaglar = await context.ProductSubCategories.FirstAsync(sc => sc.Name == "Motor Yağları", ct);

            List<Product> list =
            [
                new() { Code = "KBL-001", Name = "NYAF 4x4 Kablo", Brand = "KabloTek", CategoryId = elektrik.Id, Unit = ProductUnit.Metre, MinStock = 50 },
                new() { Code = "CIV-010", Name = "M8x40 Cıvata", Brand = "VidaSan", CategoryId = baglanti.Id, Unit = ProductUnit.Adet, MinStock = 200 },
                new() { Code = "OIL-05W30", Name = "5W30 Motor Yağı", Brand = "PetroMax", CategoryId = yaglar.Id, Unit = ProductUnit.Litre, MinStock = 100 }
            ];
            await context.Products.AddRangeAsync(list, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Ürünler eklendi.");
        }

        private static async Task SeedWarehouseStocksAsync(ApplicationDbContext context, ILogger? logger, CancellationToken ct)
        {
            if (await context.WarehouseStocks.AnyAsync(ct)) return;
            var main = await context.Warehouses.FirstAsync(w => w.Code == "DEP-001", ct);
            var mobile = await context.Warehouses.FirstAsync(w => w.Code == "DEP-002", ct);
            var cable = await context.Products.FirstAsync(p => p.Code == "KBL-001", ct);
            var bolt = await context.Products.FirstAsync(p => p.Code == "CIV-010", ct);
            var oil = await context.Products.FirstAsync(p => p.Code == "OIL-05W30", ct);

            List<WarehouseStock> list =
            [
                new() { WarehouseId = main.Id, ProductId = cable.Id, Quantity = 90, ReservedQuantity = 10, LastUpdated = DateTime.Now },
                new() { WarehouseId = main.Id, ProductId = bolt.Id, Quantity = 60, LastUpdated = DateTime.Now },
                new() { WarehouseId = mobile.Id, ProductId = bolt.Id, Quantity = 20, LastUpdated = DateTime.Now },
                new() { WarehouseId = main.Id, ProductId = oil.Id, Quantity = 350, ReservedQuantity = 50, LastUpdated = DateTime.Now }
            ];
            await context.WarehouseStocks.AddRangeAsync(list, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Depo stokları eklendi.");
        }

        private static async Task SeedRequestsAndPurchasesAsync(ApplicationDbContext context, string adminUserId, ILogger? logger, CancellationToken ct)
        {
            if (await context.Requests.AnyAsync(ct)) return;
            var main = await context.Warehouses.FirstAsync(w => w.Code == "DEP-001", ct);
            var merkez = await context.Locations.FirstAsync(l => l.Name == "Merkez Tesis", ct);
            var cable = await context.Products.FirstAsync(p => p.Code == "KBL-001", ct);
            var bolt = await context.Products.FirstAsync(p => p.Code == "CIV-010", ct);
            var enerji = await context.Suppliers.FirstAsync(s => s.CompanyName == "Enerji Market", ct);
            var mega = await context.Suppliers.FirstAsync(s => s.CompanyName == "Mega Teknik", ct);

            Request req = new()
            {
                RequestNumber = "REQ-2024-0001",
                RequestDate = DateTime.Today.AddDays(-15),
                Description = "Yeni şantiye malzeme talebi",
                WarehouseId = main.Id,
                LocationId = merkez.Id,
                RequestedByUserId = adminUserId
            };
            RequestItem i1 = new() { RequestItemNumber = "REQ-2024-0001-01", ProductId = cable.Id, RequestedQuantity = 80, Status = TalepDurumu.PurchaseApproved, ApprovalStage = OnayAsamasi.ManagementApproved, TargetWarehouseId = main.Id };
            RequestItem i2 = new() { RequestItemNumber = "REQ-2024-0001-02", ProductId = bolt.Id, RequestedQuantity = 150, Status = TalepDurumu.PurchaseApproved, ApprovalStage = OnayAsamasi.ManagementApproved, TargetWarehouseId = main.Id };
            req.Items = [i1, i2];

            Purchase po = new()
            {
                PurchaseNumber = "PO-2024-0001",
                PurchaseDate = DateTime.Today.AddDays(-12),
                Description = "Şantiye açılışı ana satınalma",
                LocationId = merkez.Id,
                CreatedByUserId = adminUserId,
                Request = req
            };
            PurchaseDetail d1 = new() { ProductId = cable.Id, SupplierId = enerji.Id, Quantity = 100, UnitPrice = 145.50m, Currency = CurrencyType.TRY, Status = TalepDurumu.PurchaseApproved, ApprovalStage = OnayAsamasi.ManagementApproved, RequestItem = i1 };
            PurchaseDetail d2 = new() { ProductId = bolt.Id, SupplierId = mega.Id, Quantity = 200, UnitPrice = 4.20m, Currency = CurrencyType.TRY, Status = TalepDurumu.PurchaseApproved, ApprovalStage = OnayAsamasi.ManagementApproved, RequestItem = i2 };
            po.Details = [d1, d2];
            await context.Purchases.AddAsync(po, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Talep ve satınalma örnekleri eklendi.");
        }

        private static async Task SeedStockMovementsAsync(ApplicationDbContext context, string adminUserId, ILogger? logger, CancellationToken ct)
        {
            if (await context.StockMovements.AnyAsync(ct)) return;
            var main = await context.Warehouses.FirstAsync(w => w.Code == "DEP-001", ct);
            var mobile = await context.Warehouses.FirstAsync(w => w.Code == "DEP-002", ct);
            var cable = await context.Products.FirstAsync(p => p.Code == "KBL-001", ct);
            var bolt = await context.Products.FirstAsync(p => p.Code == "CIV-010", ct);
            var oil = await context.Products.FirstAsync(p => p.Code == "OIL-05W30", ct);
            var dCable = await context.PurchaseDetails.FirstAsync(pd => pd.ProductId == cable.Id, ct);
            var dBolt = await context.PurchaseDetails.FirstAsync(pd => pd.ProductId == bolt.Id, ct);

            List<StockMovement> list =
            [
                new() { ProductId = cable.Id, MovementType = MovementType.In, Quantity = 100, Unit = cable.Unit, MovementDate = DateTime.Today.AddDays(-11), Description = "Satınalma teslimatı", TargetWarehouseId = main.Id, PurchaseDetailId = dCable.Id, UserId = adminUserId },
                new() { ProductId = bolt.Id, MovementType = MovementType.In, Quantity = 200, Unit = bolt.Unit, MovementDate = DateTime.Today.AddDays(-10), Description = "Satınalma teslimatı", TargetWarehouseId = main.Id, PurchaseDetailId = dBolt.Id, UserId = adminUserId },
                new() { ProductId = cable.Id, MovementType = MovementType.Out, Quantity = 10, Unit = cable.Unit, MovementDate = DateTime.Today.AddDays(-7), Description = "Şantiye ana dağıtım", SourceWarehouseId = main.Id, RequestItemId = dCable.RequestItemId, UserId = adminUserId },
                new() { ProductId = bolt.Id, MovementType = MovementType.Transfer, Quantity = 20, Unit = bolt.Unit, MovementDate = DateTime.Today.AddDays(-5), Description = "Mobil depoya transfer", SourceWarehouseId = main.Id, TargetWarehouseId = mobile.Id, UserId = adminUserId },
                new() { ProductId = oil.Id, MovementType = MovementType.In, Quantity = 400, Unit = oil.Unit, MovementDate = DateTime.Today.AddDays(-9), Description = "Bakım için yağ girişi", TargetWarehouseId = main.Id, UserId = adminUserId }
            ];
            await context.StockMovements.AddRangeAsync(list, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Stok hareketleri eklendi.");
        }

        private static async Task SeedReservationsAsync(ApplicationDbContext context, string adminUserId, ILogger? logger, CancellationToken ct)
        {
            if (await context.StockReservations.AnyAsync(ct)) return;
            var main = await context.Warehouses.FirstAsync(w => w.Code == "DEP-001", ct);
            var oil = await context.Products.FirstAsync(p => p.Code == "OIL-05W30", ct);
            StockReservation r = new()
            {
                ProductId = oil.Id,
                WarehouseId = main.Id,
                ReservedQuantity = 50,
                Type = ReservationType.Automatic,
                Status = RezervasyonDurumu.ReservationActive,
                CreatedAt = DateTime.Today.AddDays(-2),
                ReleasedAt = DateTime.Today.AddDays(5),
                CreatedByUserId = adminUserId,
                IsActive = true
            };
            await context.StockReservations.AddAsync(r, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Rezervasyon eklendi.");
        }

        private static async Task SeedFuelDataAsync(ApplicationDbContext context, ILogger? logger, CancellationToken ct)
        {
            if (await context.AkaryakitIstasyonlar.AnyAsync(ct)) return;
            List<AkaryakitIstasyon> stations =
            [
                new() { Ad = "Merkez Pompa", Tip = "Dizel" },
                new() { Ad = "Saha 1 Tank", Tip = "Benzin" }
            ];
            await context.AkaryakitIstasyonlar.AddRangeAsync(stations, ct);
            await context.SaveChangesAsync(ct);
            var merkez = stations[0];
            var saha = stations[1];

            List<AkaryakitGiris> giris =
            [
                new() { IstasyonID = merkez.IstasyonID, Tarih = DateTime.Today.AddDays(-10), MiktarLitre = 1200, Aciklama = "Dizel tanker dolumu" },
                new() { IstasyonID = saha.IstasyonID, Tarih = DateTime.Today.AddDays(-6), MiktarLitre = 600, Aciklama = "Mobil tank dolumu" }
            ];
            await context.AkaryakitGirisler.AddRangeAsync(giris, ct);

            List<AkaryakitCikis> cikis =
            [
                new() { KaynakIstasyonID = merkez.IstasyonID, Tarih = DateTime.Today.AddDays(-3), MiktarLitre = 300, HedefTip = "Arac", KM = 45210, CalismaSaati = 1220, Aciklama = "Servis aracı yakıtı" },
                new() { KaynakIstasyonID = saha.IstasyonID, Tarih = DateTime.Today.AddDays(-2), MiktarLitre = 180, HedefTip = "Jeneratör", CalismaSaati = 340, Aciklama = "Gece çalışması" }
            ];
            await context.AkaryakitCikislar.AddRangeAsync(cikis, ct);
            await context.SaveChangesAsync(ct);
            logger?.LogInformation("✅ Yakıt verileri eklendi.");
        }
    }
}
