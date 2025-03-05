using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AuthenticationModel;
using iPhoneBE.Data.ViewModels.AuthenticationVM;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace iPhoneBE.Service.Interfaces
{
    public interface IAccountServices
    {
        Task<string> CreateAccessToken(User user);
        Task<string> CreateRefreshToken(User user);
        Task<JwtViewModel> GoogleLoginAsync(GoogleAuthModel model);
        Task<JwtViewModel> LoginAsync(LoginModel model);
        Task<IdentityResult> RegisterAsync(User user);
        Task<JwtViewModel> ValidateRefreshToken(RefreshTokenModel model);
        Task ValidateToken(TokenValidatedContext context);
    }
}