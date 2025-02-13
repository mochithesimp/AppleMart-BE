using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Services
{
    public class ProductItemServices : IProductItemServices
    {
        private readonly IProductItemRepository productItemRepository;

        public ProductItemServices(IProductItemRepository productItemRepository)
        {
            this.productItemRepository = productItemRepository;
        }

        public Task<ProductItem> AddAsync(ProductItem productItem)
        {
            return productItemRepository.AddAsync(productItem);
        }

        public Task<ProductItem> DeleteAsync(int id)
        {
            return productItemRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<ProductItem>> GetAllAsync(string? productItemName)
        {
            return productItemRepository.GetAllAsync(productItemName);
        }

        public Task<ProductItem> GetByIdAsync(int id)
        {
            return productItemRepository.GetByIdAsync(id);
        }

        public Task<ProductItem> UpdateAsync(int id, ProductItem productItem)
        {
            return productItemRepository.UpdateAsync(id, productItem);
        }
    }
}
