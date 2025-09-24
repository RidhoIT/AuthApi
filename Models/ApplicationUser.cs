﻿using Microsoft.AspNetCore.Identity;

namespace AuthApi.Models
{
    public class ApplicationUser : IdentityUser

    {

        public List<RefreshToken> RefreshTokens { get; set; } = new();

    }

}
