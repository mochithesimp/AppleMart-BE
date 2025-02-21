using iPhoneBE.Data.Model;

namespace iPhoneBE.Service.Interfaces
{
    public interface IUserServices
    {
        Task<User> FindByEmail(string email);
    }
}