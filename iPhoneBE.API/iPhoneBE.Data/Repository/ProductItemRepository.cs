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
    public class ProductItemRepository : Repository<ProductItem>, IProductItemRepository
    {
        private readonly AppleMartDBContext _dbContext;
        private readonly IUnitOfWork unitOfWork;

        public ProductItemRepository(AppleMartDBContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _dbContext = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ProductItem> GetByIdAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.ProductItemID == id && !x.IsDeleted);
        }


        public async Task<IEnumerable<ProductItem>> GetAllAsync(string? producttemName)
        {
            var query = Entities.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(producttemName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(producttemName.ToLower()));
            }

            return await query.ToListAsync();
        }


        public async Task<ProductItem> AddAsync(ProductItem productItem)
        {
            if (productItem == null)
            {
                throw new ArgumentNullException(nameof(productItem));
            }

            var existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(c => c.ProductID == productItem.ProductID);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with Id {productItem.ProductID} not found.");
            }

            if (!string.IsNullOrEmpty(productItem.Color) && !productItem.Color.All(char.IsLetter))
            {
                throw new ArgumentException("Color must contain only letters.");
            }

            var existProductItem = await Entities.FirstOrDefaultAsync(c => c.Name.Equals(productItem.Name));
            if (existProductItem != null)
            {
                throw new Exception($"Product {productItem.Name} is existed!");
            }

            await Entities.AddAsync(productItem);
            await _dbContext.SaveChangesAsync();
            return productItem;
        }

        public async Task<ProductItem> UpdateAsync(int id, ProductItem productItem)
        {
            if (productItem == null)
            {
                throw new ArgumentNullException(nameof(productItem));
            }

            var existingProductItem = await GetByIdAsync(id);
            if (existingProductItem == null)
            {
                throw new KeyNotFoundException($"Product Item with Id {id} not found.");
            }

            if (productItem.ProductID != 0)
            {
                var existingProduct = await _dbContext.Products
                    .FirstOrDefaultAsync(c => c.ProductID == productItem.ProductID);
                if (existingProduct == null)
                {
                    throw new KeyNotFoundException($"Product with Id {productItem.ProductID} not found.");
                }
                existingProductItem.ProductID = productItem.ProductID;
            }

            existingProductItem.Name = (productItem.Name == "string" || string.IsNullOrWhiteSpace(productItem.Name))
                ? existingProductItem.Name
                : productItem.Name;

            existingProductItem.Description = (productItem.Description == "string" || string.IsNullOrWhiteSpace(productItem.Description))
                ? existingProductItem.Description
                : productItem.Description;

            existingProductItem.Color = (productItem.Color == "string" || string.IsNullOrWhiteSpace(productItem.Color))
                ? existingProductItem.Color
                : productItem.Color;

            existingProductItem.Quantity = productItem.Quantity <= 0
                ? existingProductItem.Quantity
                : productItem.Quantity;

            existingProductItem.Price = productItem.Price <= 0
                ? existingProductItem.Price
                : productItem.Price;

            await _dbContext.SaveChangesAsync();
            return existingProductItem;
        }


        public async Task<ProductItem> DeleteAsync(int id)
        {
            var productItem = await GetByIdAsync(id);
            if (productItem == null)
            {
                throw new KeyNotFoundException($"Product Item with Id {id} not found.");
            }

            productItem.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return productItem;
        }
    }
}
