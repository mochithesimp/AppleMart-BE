using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductItemModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IProductItemServices
    {
        Task<List<ProductItem>> GetAllWithoutFilter();
        Task<PagedResult<ProductItem>> GetAllAsync(ProductItemFilterModel filter);
        Task<ProductItem> GetByIdAsync(int id);
        Task<ProductItem> AddAsync(ProductItem productItem);
        Task<ProductItem> UpdateAsync(int id, UpdateProductItemModel newProductItem);
        Task<ProductItem> DeleteAsync(int id);
        Task<object> GetTotalSoldForProductItemAsync(int productItemId);
    }
}