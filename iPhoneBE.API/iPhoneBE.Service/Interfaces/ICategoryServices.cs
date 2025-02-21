using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.CategoryModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface ICategoryServices
    {
        Task<Category> AddAsync(Category category);
        Task<Category> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync(string? categoryName);
        Task<Category> GetByIdAsync(int id);
        Task<Category> UpdateAsync(int id, UpdateCategoryModel newCategory);
    }
}