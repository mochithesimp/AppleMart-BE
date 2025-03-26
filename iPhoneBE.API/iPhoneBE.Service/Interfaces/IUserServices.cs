using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.UserVM;

namespace iPhoneBE.Service.Interfaces
{
    public interface IUserServices
    {
        Task<UserViewModel> DeleteAsync(string id);
        Task<User> FindByEmail(string email);
        Task<User> GetByIdAsync(string id);
        Task<IEnumerable<UserViewModel>> GetAllAsync(string role);
        Task<IEnumerable<User>> GetShippersAsync();
        Task<(User user, string role)> GetUserWithRoleAsync(string id);
        Task<UserViewModel> UpdateAsync(string id, UserModel updatedUser);
        Task<UserViewModel> ChangeUserRoleAsync(string userId, string newRole);
    }
}