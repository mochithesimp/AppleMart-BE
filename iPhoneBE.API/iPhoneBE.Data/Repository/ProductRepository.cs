using iPhoneBE.Data.Data;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private readonly AppleMartDBContext _dbContext;
        private readonly IUnitOfWork unitOfWork;

        public ProductRepository(AppleMartDBContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _dbContext = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.ProductID == id && !x.IsDeleted);
        }


        public async Task<IEnumerable<Product>> GetAllAsync(string? productName)
        {
            var query = Entities.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(productName.ToLower()));
            }

            return await query.ToListAsync();
        }


        public async Task<Product> AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var existingCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == product.CategoryID);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with Id {product.CategoryID} not found.");
            }

            var existProduct = await Entities.FirstOrDefaultAsync(c => c.Name.Equals(product.Name));
            if (existProduct != null)
            {
                throw new Exception($"Product {product.Name} is existed!");
            }

            await Entities.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(int id, Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var existingProduct = await GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with Id {id} not found.");
            }

            if (product.CategoryID != 0) 
            {
                var existingCategory = await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.CategoryID == product.CategoryID);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException($"Category with Id {product.CategoryID} not found.");
                }
                existingProduct.CategoryID = product.CategoryID;
            }

            existingProduct.Name = string.IsNullOrWhiteSpace(product.Name) || product.Name == "string"
                ? existingProduct.Name
                : product.Name;

            existingProduct.Description = string.IsNullOrWhiteSpace(product.Description) || product.Description == "string"
                ? existingProduct.Description
                : product.Description;

            await _dbContext.SaveChangesAsync();
            return existingProduct;
        }


        public async Task<Product> DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with Id {id} not found.");
            }
            product.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return product;
        }
    }
}
