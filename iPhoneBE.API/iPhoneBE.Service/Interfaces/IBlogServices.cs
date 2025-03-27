using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.BlogModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IBlogServices
    {
        Task<Blog> AddAsync(Blog blog);
        Task<Blog> DeleteAsync(int id);
        Task<IEnumerable<Blog>> GetAllAsync(string searchTitle = null);
        Task<Blog> GetByIdAsync(int id);
        Task<Blog> UpdateAsync(int id, UpdateBlogModel newBlog);
    }
}