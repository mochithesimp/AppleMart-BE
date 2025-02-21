﻿using iPhoneBE.Data.Models.AuthenticationModel;
using iPhoneBE.Data.ViewModels.AuthenticationDTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace iPhoneBE.Service.Interfaces
{
    public interface IAccountServices
    {
        Task<JwtViewModel> LoginAsync(LoginModel model);
        Task<IdentityResult> RegisterAsync(RegisterModel model);
        Task<JwtViewModel> ValidateRefreshToken(RefreshTokenModel model);
        Task ValidateToken(TokenValidatedContext context);
    }
}