using AutoMapper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.UserVM;
using iPhoneBE.Service.Extensions;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Services
{
    public class UserServices : IUserServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserServices(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync(string role)
        {
            var usersQuery = _userManager.Users.Where(u => !u.IsDeleted);

            var users = await usersQuery.FilterByRoleAsync(role, _userManager);

            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault() ?? "No Role";

                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    Role = userRole
                });
            }

            return userList;
        }




        public async Task<(User user, string role)> GetUserWithRoleAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.FirstOrDefault() ?? "No Role";

            return (user, role);
        }


        public async Task<User> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<UserViewModel> UpdateAsync(string id, UserModel updatedUser)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");

                _mapper.Map(updatedUser, user);

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Failed to update user.");
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var userRoles = await _userManager.GetRolesAsync(user);

                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.Role = userRoles.FirstOrDefault() ?? "No Role";

                return userViewModel;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        //soft deleted
        public async Task<UserViewModel> DeleteAsync(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");

                user.IsDeleted = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Failed to delete user.");
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var userRoles = await _userManager.GetRolesAsync(user);

                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.Role = userRoles.FirstOrDefault() ?? "No Role";

                return userViewModel;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
