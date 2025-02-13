using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Interfaces
{
    public interface IProductItemRepository
    {
        Task<ProductItem> GetByIdAsync(int id);
        Task<IEnumerable<ProductItem>> GetAllAsync(string? producttemName);
        Task<ProductItem> AddAsync(ProductItem productItem);
        Task<ProductItem> UpdateAsync(int id, ProductItem productItem);
        Task<ProductItem> DeleteAsync(int id);
    }
}
