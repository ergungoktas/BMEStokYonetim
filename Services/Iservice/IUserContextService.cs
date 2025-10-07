namespace BMEStokYonetim.Services.Iservice
{
    public interface IUserContextService
    {
        string? GetUserId();  // senkron erişim
        Task<string?> GetCurrentUserIdAsync(); // async erişim
    }


}
