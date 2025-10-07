using BMEStokYonetim.Data.Entities;

namespace BMEStokYonetim.Services.Iservice
{
    public interface IRequestService
    {
        // RequestNumber üretimi (BME-Tlp-LOC-YY-00001 formatı)
        Task<string> GenerateRequestNumberAsync(int locationId);

        // RequestItemNumber üretimi (RequestNumber-01, -02 şeklinde)
        Task AssignRequestItemNumbers(Request request);
    }
}
