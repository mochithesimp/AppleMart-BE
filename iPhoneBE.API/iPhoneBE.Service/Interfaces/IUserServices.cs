using iPhoneBE.Data.Model;
using iPhoneBE.Data.ViewModels.UserDTO;

namespace iPhoneBE.Service.Interfaces
{
    public interface IUserServices
    {
        Task<User> FindByEmail(string email);
        Task<UserViewModel> GetUserByID(string id);
    }
}