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
        private readonly IRepository<Category> _repository;

        public CategoryServices(IRepository<Category> repository)
        {
            _repository = repository;
        }

        public async Task<Category> AddAsync(Category category)
        {
            var result = await _repository.AddAsync(category);

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            bool result = false;

            Category category = await _repository.GetByIdAsync(id);

            if (category == null)
            {
                throw new KeyNotFoundException("404 - Product not found.");
            }

            result = await _repository.SoftDelete(category);
            await _repository.CommitAsync();

            return result;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(string? categoryName)
        {
            var result = await _repository.GetAllAsync(categoryName == null ? null : c => c.Name == categoryName);
            result = result.Where(r => r.IsDeleted == false);
            return result;
        }

        public Task<Category> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, Category category)
        {
            var newCategory = await _repository.GetByIdAsync(id);
            var result = false;
            if (newCategory == null)
            {
                throw new KeyNotFoundException("404 - Product not found.");
            }
            result = await _repository.Update(newCategory);
            return result;
        }
    }
}
