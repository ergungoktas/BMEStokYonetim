﻿
using Microsoft.AspNetCore.Identity;
namespace BMEStokYonetim.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
