using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AuthenticationModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Interfaces
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> RegisterAsync(RegisterModel model);
        public Task<User> LoginAsync(LoginModel model);
    }
}
