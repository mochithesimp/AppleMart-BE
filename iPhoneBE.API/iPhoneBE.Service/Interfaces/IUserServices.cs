using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.UserDTO;

namespace iPhoneBE.Service.Interfaces
{
    public interface IUserServices
    {
        Task<UserViewModel> DeleteAsync(string id);
        Task<User> FindByEmail(string email);
        Task<IEnumerable<UserViewModel>> GetAllAsync();
        Task<UserViewModel> GetByIdAsync(string id);
        Task<UserViewModel> UpdateAsync(string id, UserModel updatedUser);
    }
}