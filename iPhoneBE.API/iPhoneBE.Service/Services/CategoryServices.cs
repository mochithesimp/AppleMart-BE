using AutoMapper;
using iPhoneBE.Data;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.CategoryModel;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //get all
        public async Task<IEnumerable<Category>> GetAllAsync(string? categoryName)
        {
            var result = await _unitOfWork.CategoryRepository.GetAllAsync(categoryName == null ? null : c => c.Name == categoryName);

            result = result?.Where(r => r.IsDeleted == false) ?? new List<Category>();

            return result;
        }

        //get by id
        public async Task<Category> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found");

            return category;
        }

        public async Task<Category> AddAsync(Category category)
        {
            _unitOfWork.BeginTransaction();

            try
            {
                var result = await _unitOfWork.CategoryRepository.AddAsync(category);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return result;
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        //update
        public async Task<Category> UpdateAsync(int id, UpdateCategoryModel newCategory)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                    throw new KeyNotFoundException($"Category with ID {id} not found.");

                _mapper.Map(newCategory, category);
                var result = await _unitOfWork.CategoryRepository.Update(category);

                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to update category.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return category;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }


        //soft delete
        public async Task<Category> DeleteAsync(int id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                    throw new KeyNotFoundException($"Category with ID {id} not found.");

                var result = await _unitOfWork.CategoryRepository.SoftDelete(category);
                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to delete category.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return category;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

    }
}
