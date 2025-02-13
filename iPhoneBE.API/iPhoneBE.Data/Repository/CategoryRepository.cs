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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppleMartDBContext _dbContext;
        private readonly IUnitOfWork unitOfWork;

        public CategoryRepository(AppleMartDBContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _dbContext = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.CategoryID == id && !x.IsDeleted);
        }


        public async Task<IEnumerable<Category>> GetAllAsync(string? categoryName)
        {
            var query = Entities.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(categoryName.ToLower()));
            }

            return await query.ToListAsync();
        }


        public async Task<Category> AddAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
      
            var existCategory = await Entities.FirstOrDefaultAsync(c => c.Name.Equals(category.Name));
            if (existCategory != null)
            {
                throw new Exception($"Category {category.Name} is existed!");
            }

            await Entities.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(int id, Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var existingCategory = await GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }

            existingCategory.Name = string.IsNullOrWhiteSpace(category.Name) || category.Name == "string"
                ? existingCategory.Name
                : category.Name;

            existingCategory.Description = string.IsNullOrWhiteSpace(category.Description) || category.Description == "string"
                ? existingCategory.Description
                : category.Description;

            await _dbContext.SaveChangesAsync();
            return existingCategory;
        }


        public async Task<Category> DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with Id {id} not found.");
            }
            category.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return category;
        }
    }
}
