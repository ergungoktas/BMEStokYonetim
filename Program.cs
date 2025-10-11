using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.BackgroundJobs;
using BMEStokYonetim.Services.Iservice;
using BMEStokYonetim.Services.Service;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ✅ Güvenli bağlantı kontrolü
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection bağlantı dizesi yapılandırılmamış.");
}

// ✅ DbContext ve Factory kaydı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpContextAccessor();

// ✅ Data Protection (kalıcı anahtar deposu)
string keyPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "BMEStokYonetim_Keys");
builder.Services.AddScoped<ExcelImportService>(); // ✅ Bu satırı ekle
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
    .SetApplicationName("BMEStokYonetim");

// ✅ Servis kayıtları
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
builder.Services.AddScoped<IFuelService, FuelService>();
builder.Services.AddScoped<IAssetDailyCheckService, AssetDailyCheckService>();


// ✅ Razor ve Blazor ayarları
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// ✅ Logging ayarları
builder.Services.AddLogging(logging =>
{
    _ = logging.ClearProviders();
    _ = logging.AddConsole();
    _ = logging.AddDebug();
    _ = logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
});

// ✅ Identity ayarları
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

// ✅ Cookie yönlendirme hatası engelleme
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    };
});

// ✅ Quartz job (stok rezervasyon yenileme)
builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new("ReservationJob");
    _ = q.AddJob<ReservationJob>(opts => opts.WithIdentity(jobKey));
    _ = q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReservationJob-trigger")
        .WithCronSchedule("0 5 0 * * ?")); // Her gün 00:05'te
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

WebApplication app = builder.Build();

// ✅ Database migration & seeding
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    IDbContextFactory<ApplicationDbContext> factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await using ApplicationDbContext db = await factory.CreateDbContextAsync();

    try
    {
        await db.Database.MigrateAsync();
        logger.LogInformation("✅ Database migration completed successfully.");
        logger.LogInformation("🔗 Connected DB: {Database}", db.Database.GetDbConnection().Database);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Migration error: {Message}", ex.Message);
    }
}
else
{
    _ = app.UseExceptionHandler("/Error");
    _ = app.UseHsts();
}

// ✅ Uygulama yapılandırması
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
