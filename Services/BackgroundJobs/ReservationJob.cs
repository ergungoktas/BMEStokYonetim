<<<<<<< ours
﻿using BMEStokYonetim.Services.Iservice;
using Microsoft.Extensions.DependencyInjection;
=======
using BMEStokYonetim.Services.Iservice;
>>>>>>> theirs
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
<<<<<<< ours
        private readonly IServiceScopeFactory _scopeFactory;

        public ReservationJob(ILogger<ReservationJob> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
=======
        private readonly IReservationService _reservationService;

        public ReservationJob(ILogger<ReservationJob> logger, IReservationService reservationService)
        {
            _logger = logger;
            _reservationService = reservationService;
>>>>>>> theirs
        }

        public async Task Execute(IJobExecutionContext context)
        {
<<<<<<< ours
            _logger.LogInformation("🔹 Rezervasyon Job çalıştı: {time}", DateTimeOffset.Now);

            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            IReservationService reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

            try
            {
                ReservationProcessResult result = await reservationService.ProcessExpiredReservationsAsync();

                if (result.TotalProcessed == 0)
                {
                    _logger.LogInformation("⏳ Süresi dolan rezervasyon bulunamadı ({time})", DateTimeOffset.Now);
=======
            DateTimeOffset startedAt = DateTimeOffset.UtcNow;
            _logger.LogInformation("Reservation job started at {StartedAt:u}", startedAt);

            try
            {
                ReservationProcessResult result = await _reservationService.ProcessExpiredReservationsAsync();

                if (result.TotalProcessed == 0)
                {
                    _logger.LogInformation(
                        "Reservation job completed at {CompletedAt:u}. No reservations required processing.",
                        DateTimeOffset.UtcNow);
>>>>>>> theirs
                    return;
                }

                _logger.LogInformation(
<<<<<<< ours
                    "🎯 Rezervasyon jobu tamamlandı. İşlenen: {total}, Süresi Dolan: {expired}. Mesaj: {message}",
                    result.TotalProcessed,
                    result.ExpiredReservations,
                    result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Rezervasyon jobu çalıştırılırken hata oluştu.");
=======
                    "Reservation job completed at {CompletedAt:u}. Processed {Total} reservations, expired {Expired}. Message: {Message}",
                    DateTimeOffset.UtcNow,
                    result.TotalProcessed,
                    result.ExpiredReservations,
                    string.IsNullOrWhiteSpace(result.Message) ? "(no details)" : result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reservation job failed at {FailedAt:u}", DateTimeOffset.UtcNow);
>>>>>>> theirs
                throw;
            }
        }
    }
}
