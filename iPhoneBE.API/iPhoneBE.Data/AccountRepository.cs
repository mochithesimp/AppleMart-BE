using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AuthenticationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> LoginAsync(LoginModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, model.Password);

            if (existingUser == null || !isCorrect)
            {
                return null;
            }

            return existingUser;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            var newUser = new User
            {
                Name = model.name,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                

                //add role
                if(!await _roleManager.RoleExistsAsync(RolesHelper.Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(RolesHelper.Customer));
                }
                await _userManager.AddToRoleAsync(newUser, RolesHelper.Customer);
            }

            return result;
        }
    }
}
