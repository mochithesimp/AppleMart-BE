using iPhoneBE.Data.Models.AuthenticationModel;
using Microsoft.AspNetCore.Identity;

namespace iPhoneBE.Service.Interfaces
{
    public interface IAccountServices
    {
        Task<string> LoginAsync(LoginModel model);
        Task<IdentityResult> RegisterAsync(RegisterModel model);
    }
}