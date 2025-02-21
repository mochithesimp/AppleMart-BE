using AutoMapper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.UserDTO;
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

        public UserServices(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor , IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            var claimsUser = _httpContextAccessor.HttpContext?.User; // Lấy ClaimsPrincipal từ HttpContext
            if (claimsUser == null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user context.");
            }

            var currentUserRole = claimsUser.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(currentUserRole))
            {
                throw new UnauthorizedAccessException("User role not found.");
            }

            //if (currentUserRole != "Admin" && currentUserRole != "Staff")
            //{
            //    throw new UnauthorizedAccessException($"Access denied for role: {currentUserRole}");
            //}

            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userRole = userRoles.FirstOrDefault() ?? "No Role";

                //if (currentUserRole == "Staff" && (userRole == "Admin" || userRole == "Staff"))
                //{
                //    continue; // Nếu user hiện tại là Staff, loại bỏ các user có role Admin hoặc Staff
                //}

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


        public async Task<UserViewModel> GetByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.Role = userRoles.FirstOrDefault() ?? "No Role";

                return userViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<UserViewModel> UpdateAsync(string id, UserModel updatedUser)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");

                _mapper.Map(updatedUser, user);

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to update user.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                var userRoles = await _userManager.GetRolesAsync(user);

                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.Role = userRoles.FirstOrDefault() ?? "No Role";

                return userViewModel;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        //soft deleted
        public async Task<UserViewModel> DeleteAsync(string id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");

                user.IsDeleted = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to delete user.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                var userRoles = await _userManager.GetRolesAsync(user);

                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.Role = userRoles.FirstOrDefault() ?? "No Role";

                return userViewModel;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }
    }
}
