using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IProductServices
    {
        Task<Product> AddAsync(Product product);
        Task<Product> DeleteAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> UpdateAsync(int id, UpdateProductModel newProduct);
    }
}