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
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository productRepository;

        public ProductServices(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public Task<Product> AddAsync(Product product)
        {
            return productRepository.AddAsync(product);
        }

        public Task<Product> DeleteAsync(int id)
        {
            return productRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Product>> GetAllAsync(string? productName)
        {
            return productRepository.GetAllAsync(productName);
        }

        public Task<Product> GetByIdAsync(int id)
        {
            return productRepository.GetByIdAsync(id);
        }

        public Task<Product> UpdateAsync(int id, Product product)
        {
            return productRepository.UpdateAsync(id, product);
        }
    }
}
