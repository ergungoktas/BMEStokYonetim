using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BMEStokYonetim.Services.Service
{
    public class UserContextService : IUserContextService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Kullanıcının senkron Id bilgisini Claims üzerinden döndürür.
        /// </summary>
        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Kullanıcı Id'sini async olarak döndürür (Identity User tablosundan).
        /// </summary>
        public async Task<string?> GetCurrentUserIdAsync()
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;
            if (principal == null)
            {
                return null;
            }

            ApplicationUser? user = await _userManager.GetUserAsync(principal);
            return user?.Id;
        }
    }
}
