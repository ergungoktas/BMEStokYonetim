using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.BackgroundJobs;
using BMEStokYonetim.Services.Iservice;
using BMEStokYonetim.Services.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ✅ 1. DbContext & Factory kaydı (tek ve güvenli yapı)
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpContextAccessor();

// ✅ 2. Servis kayıtları
builder.Services.AddScoped<IStockReportService, StockReportService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();

// ✅ 3. Logging ayarları
builder.Services.AddLogging(logging =>
{
    _ = logging.ClearProviders();
    _ = logging.AddConsole();
    _ = logging.AddDebug();
    _ = logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
});

// ✅ 4. Identity konfigürasyonu
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ✅ 5. Quartz Job (ReservationJob)
builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new("ReservationJob");
    _ = q.AddJob<ReservationJob>(opts => opts.WithIdentity(jobKey));

    // Her gece 00:05'te çalışır
    _ = q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReservationJob-trigger")
        .WithCronSchedule("0 5 0 * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ✅ 6. Kestrel (HTTP)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000);
});

WebApplication app = builder.Build();

// ✅ 7. Migration sadece (Seeder yok)
using (IServiceScope scope = app.Services.CreateScope())
{
    IDbContextFactory<ApplicationDbContext> factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    await using ApplicationDbContext db = await factory.CreateDbContextAsync();
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await db.Database.MigrateAsync();
        logger.LogInformation("✅ Database migration completed successfully.");
        logger.LogInformation($"🔗 Connected DB: {db.Database.GetDbConnection().ConnectionString}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Migration error: {Message}", ex.Message);
    }
}

// ✅ 8. Uygulama başladığında log
IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
    logger.LogInformation("🚀 Uygulama başlatıldı - Tüm servisler aktif. Quartz Job gece 00:05'te çalışacak.");
});

// ✅ 9. Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
    _ = app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
