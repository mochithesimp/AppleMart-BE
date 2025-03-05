using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Models.ProductItemAttributeModel;
using iPhoneBE.Service.Interfaces;

namespace iPhoneBE.Service.Services
{
    public class ProductItemAttributeServices : IProductItemAttributeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductItemAttributeServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductItemAttribute>> GetAllAsync()
        {
            var result = await _unitOfWork.ProductItemAttributeRepository.GetAllAsync();

            result = result?.Where(r => r.IsDeleted == false) ?? new List<ProductItemAttribute>();

            return result;
        }

        public async Task<ProductItemAttribute> GetByIdAsync(int id)
        {
            var productItemAttribute = await _unitOfWork.ProductItemAttributeRepository.GetByIdAsync(id);
            if (productItemAttribute == null)
                throw new KeyNotFoundException("ProductItemAttribute not found");

            return productItemAttribute;
        }

        public async Task<ProductItemAttribute> AddAsync(ProductItemAttribute productItemAttribute)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemAttribute.ProductItemID);
                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {productItemAttribute.ProductItemID} not found.");

                var attribute = await _unitOfWork.AttributeRepository.GetByIdAsync(productItemAttribute.AttributeID);
                if (attribute == null)
                    throw new KeyNotFoundException($"Attribute with ID {productItemAttribute.AttributeID} not found.");

                var result = await _unitOfWork.ProductItemAttributeRepository.AddAsync(productItemAttribute);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return result;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ProductItemAttribute> UpdateAsync(int id, UpdateProductItemAttributeModel newProductItemAttribute)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var productItemAttribute = await _unitOfWork.ProductItemAttributeRepository.GetByIdAsync(id);
                if (productItemAttribute == null)
                    throw new KeyNotFoundException($"ProductItemAttribute with ID {id} not found.");

                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(newProductItemAttribute.ProductItemID);
                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {newProductItemAttribute.ProductItemID} not found.");

                var attribute = await _unitOfWork.AttributeRepository.GetByIdAsync(newProductItemAttribute.AttributeID);
                if (attribute == null)
                    throw new KeyNotFoundException($"Attribute with ID {newProductItemAttribute.AttributeID} not found.");

                bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

                productItemAttribute.ProductItemID = newProductItemAttribute.ProductItemID;
                productItemAttribute.AttributeID = newProductItemAttribute.AttributeID;
                if (!IsInvalid(newProductItemAttribute.Value))
                {
                    productItemAttribute.Value = newProductItemAttribute.Value;
                }

                var result = await _unitOfWork.ProductItemAttributeRepository.Update(productItemAttribute);

                if (!result)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new InvalidOperationException("Failed to update product item attribute.");
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return productItemAttribute;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ProductItemAttribute> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var productItemAttribute = await _unitOfWork.ProductItemAttributeRepository.GetByIdAsync(id);
                if (productItemAttribute == null)
                    throw new KeyNotFoundException($"ProductItemAttribute with ID {id} not found.");

                var result = await _unitOfWork.ProductItemAttributeRepository.SoftDelete(productItemAttribute);
                if (!result)
                {
                    throw new InvalidOperationException("Failed to delete product item attribute.");
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return productItemAttribute;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}