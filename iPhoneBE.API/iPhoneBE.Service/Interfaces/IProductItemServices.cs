using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductItemModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IProductItemServices
    {
        Task<ProductItem> AddAsync(ProductItem productItem);
        Task<ProductItem> DeleteAsync(int id);
        Task<IEnumerable<ProductItem>> GetAllAsync();
        Task<ProductItem> GetByIdAsync(int id);
        Task<ProductItem> UpdateAsync(int id, UpdateProductItemModel newProductItem);
    }
}