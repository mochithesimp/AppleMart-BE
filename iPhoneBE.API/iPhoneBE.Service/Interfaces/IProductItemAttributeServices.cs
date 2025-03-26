using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.ProductItemAttributeModel;
using iPhoneBE.Data.Models.ProductItemModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IProductItemAttributeServices
    {
        Task<PagedResult<ProductItemAttribute>> GetAllAsync(ProductItemAttributeFilterModel filter);
        Task<ProductItemAttribute> GetByIdAsync(int id);
        Task<ProductItemAttribute> AddAsync(ProductItemAttribute productItemAttribute);
        Task<ProductItemAttribute> UpdateAsync(int id, UpdateProductItemAttributeModel newProductItemAttribute);
        Task<ProductItemAttribute> DeleteAsync(int id);
    }
}