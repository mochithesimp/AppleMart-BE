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
    public class CategoryServices : ICategoryServices
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryServices(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public Task<Category> AddAsync(Category category)
        {
            return categoryRepository.AddAsync(category);
        }

        public Task<Category> DeleteAsync(int id)
        {
            return categoryRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Category>> GetAllAsync(string? categoryName)
        {
            return categoryRepository.GetAllAsync(categoryName);
        }

        public Task<Category> GetByIdAsync(int id)
        {
            return categoryRepository.GetByIdAsync(id);
        }

        public Task<Category> UpdateAsync(int id, Category category)
        {
            return categoryRepository.UpdateAsync(id, category);
        }
    }
}
