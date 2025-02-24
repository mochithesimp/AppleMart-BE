using iPhoneBE.Data.Entities;

namespace iPhoneBE.Service.Interfaces
{
    public interface IBlogServices
    {
        Task<IEnumerable<Blog>> GetAllAsync();
    }
}