using iPhoneBE.Data.Model;

namespace iPhoneBE.Service.Interfaces
{
    public interface ICategoryServices
    {
        Task<Category> AddAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync(string? categoryName);
        Task<Category> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Category category);
    }
}