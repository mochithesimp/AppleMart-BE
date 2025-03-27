using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductModel;
using iPhoneBE.Data.Models.ProductItemModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IProductServices
    {
        Task<List<Product>> GetAllWithoutFilter();
        Task<PagedResult<Product>> GetAllAsync(ProductFilterModel filter);
        Task<Product> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(int id, UpdateProductModel newProduct);
        Task<Product> DeleteAsync(int id);
    }
}