using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.BackgroundJobs;
using BMEStokYonetim.Services.Iservice;
using BMEStokYonetim.Services.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

<<<<<<< ours
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection bağlantı dizesi yapılandırılmamış.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
=======
// ✅ 1. DbContext & Factory kaydı (tek ve güvenli yapı)
Action<DbContextOptionsBuilder> dbContextOptions = options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptions);
builder.Services.AddDbContextFactory<ApplicationDbContext>(dbContextOptions);
>>>>>>> theirs

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpContextAccessor();

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

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
});

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

builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new("ReservationJob");
    q.AddJob<ReservationJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReservationJob-trigger")
        .WithCronSchedule("0 5 0 * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
<<<<<<< ours
    app.UseMigrationsEndPoint();
=======
    IDbContextFactory<ApplicationDbContext> factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    await using ApplicationDbContext db = await factory.CreateDbContextAsync();
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

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
>>>>>>> theirs
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
