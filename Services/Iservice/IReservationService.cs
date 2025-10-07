using BMEStokYonetim.Data.Entities;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IReservationService
    {
        // 🔹 Otomatik rezervasyon (satınalma sonrası)
        Task CreateAutoReservationAsync(int requestItemId, int quantity);

        // 🔹 Manuel rezervasyon (birim veya depo oluşturur)
        Task CreateManualReservationAsync(int requestItemId, int quantity, DateOnly expiryDate);

        // 🔹 Rezervasyonu tamamlandı olarak işaretle
        Task CompleteReservationAsync(int reservationId);

        // 🔹 Rezervasyonu iptal et
        Task CancelReservationAsync(int reservationId);

        // 🔹 Aktif rezervasyonları getir
        Task<List<StockReservation>> GetActiveReservationsAsync();

        // 🔹 Süresi dolmuş rezervasyonları işle (expire)
        Task<ReservationProcessResult> ProcessExpiredReservationsAsync();

        // 🔹 Belirli bir tarihte dolacak rezervasyonları getir
        Task<List<StockReservation>> GetExpiringReservationsAsync(DateOnly date);
    }

    public class ReservationProcessResult
    {
        public int TotalProcessed { get; set; }
        public int ExpiredReservations { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
