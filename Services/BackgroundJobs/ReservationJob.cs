using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BMEStokYonetim.Services.BackgroundJobs
{
    /// <summary>
    /// Quartz job responsible for processing expired manual reservations and releasing stock.
    /// </summary>
    public class ReservationJob : IJob
    {
        private readonly ILogger<ReservationJob> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ReservationJob(ILogger<ReservationJob> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("🔹 Rezervasyon jobu çalıştı: {StartedAt:u}", DateTimeOffset.UtcNow);

            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            IReservationService reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

            try
            {
                ReservationProcessResult result = await reservationService.ProcessExpiredReservationsAsync();

                if (result.TotalProcessed == 0)
                {
                    _logger.LogInformation("⏳ Süresi dolan rezervasyon bulunamadı ({CompletedAt:u})", DateTimeOffset.UtcNow);
                    return;
                }

                _logger.LogInformation(
                    "🎯 Rezervasyon jobu tamamlandı ({CompletedAt:u}). İşlenen: {Total}, Süresi Dolan: {Expired}. Mesaj: {Message}",
                    DateTimeOffset.UtcNow,
                    result.TotalProcessed,
                    result.ExpiredReservations,
                    string.IsNullOrWhiteSpace(result.Message) ? "(detay yok)" : result.Message
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Rezervasyon jobu çalıştırılırken hata oluştu ({FailedAt:u}).", DateTimeOffset.UtcNow);
                throw;
            }
        }
    }
}
