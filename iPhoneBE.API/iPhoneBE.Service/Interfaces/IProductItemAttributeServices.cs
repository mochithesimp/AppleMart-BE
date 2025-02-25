using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.ProductItemAttributeModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IProductItemAttributeServices
    {
        Task<ProductItemAttribute> AddAsync(ProductItemAttribute productItemAttribute);
        Task<ProductItemAttribute> DeleteAsync(int id);
        Task<IEnumerable<ProductItemAttribute>> GetAllAsync();
        Task<ProductItemAttribute> GetByIdAsync(int id);
        Task<ProductItemAttribute> UpdateAsync(int id, UpdateProductItemAttributeModel newProductItemAttribute);
    }
}