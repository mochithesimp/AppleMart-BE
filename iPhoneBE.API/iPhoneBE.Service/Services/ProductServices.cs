using AutoMapper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductModel;
using iPhoneBE.Service.Interfaces;

namespace iPhoneBE.Service.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var result = await _unitOfWork.ProductRepository.GetAllAsync();

            result = result?
                .Where(r => r.IsDeleted == false)
                .OrderBy(r => r.DisplayIndex)
                .ToList() ?? new List<Product>();

            return result;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            return product;
        }

        public async Task<Product> AddAsync(Product product)
        {
            _unitOfWork.BeginTransaction();

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryID);
                if (category == null)
                    throw new KeyNotFoundException($"Category with ID {product.CategoryID} not found.");

                var result = await _unitOfWork.ProductRepository.AddAsync(product);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return result;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<Product> UpdateAsync(int id, UpdateProductModel newProduct)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {id} not found.");

                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(newProduct.CategoryID);
                if (category == null)
                    throw new KeyNotFoundException($"Category with ID {newProduct.CategoryID} not found.");

                bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

                product.CategoryID = newProduct.CategoryID;
                if (!IsInvalid(newProduct.Name))
                {
                    product.Name = newProduct.Name;
                }
                if (!IsInvalid(newProduct.Description))
                {
                    product.Description = newProduct.Description;
                }
                if (newProduct.DisplayIndex.HasValue)
                {
                    product.DisplayIndex = newProduct.DisplayIndex.Value;
                }

                var result = await _unitOfWork.ProductRepository.Update(product);

                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to update product.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return product;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<Product> DeleteAsync(int id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {id} not found.");

                var result = await _unitOfWork.ProductRepository.SoftDelete(product);
                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to delete product.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return product;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }
    }
}