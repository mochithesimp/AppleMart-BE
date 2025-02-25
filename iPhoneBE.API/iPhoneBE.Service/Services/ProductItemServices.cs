using AutoMapper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace iPhoneBE.Service.Services
{
    public class ProductItemServices : IProductItemServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductItemServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductItem>> GetAllAsync()
        {
            var result = await _unitOfWork.ProductItemRepository.GetAllAsync(
                o => o.IsDeleted == false,
                o => o.ProductImgs.Where(img => !img.IsDeleted),
                o => o.Product,
                o => o.ProductItemAttributes
            );

            result = result?
                    .Where(r => r.IsDeleted == false)
                    .OrderBy(r => r.DisplayIndex)
                    .ToList() ?? new List<ProductItem>();

            return result;
        }

        public async Task<ProductItem> GetByIdAsync(int id)
        {
            var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(id,
                o => o.ProductImgs.Where(img => !img.IsDeleted),
                o => o.Product,
                o => o.ProductItemAttributes);

            if (productItem == null)
                throw new KeyNotFoundException("ProductItem not found");

            return productItem;
        }

        public async Task<ProductItem> AddAsync(ProductItem productItem)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productItem.ProductID);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {productItem.ProductID} not found.");

                var images = productItem.ProductImgs?.ToList() ?? new List<ProductImg>();
                productItem.ProductImgs = new List<ProductImg>();

                var result = await _unitOfWork.ProductItemRepository.AddAsync(productItem);
                _unitOfWork.SaveChanges();

                if (images.Any())
                {
                    foreach (var img in images)
                    {
                        img.ProductItemID = result.ProductItemID;
                        await _unitOfWork.ProductImgRepository.AddAsync(img);
                    }
                    _unitOfWork.SaveChanges();
                }

                _unitOfWork.CommitTransaction();

                return await _unitOfWork.ProductItemRepository.GetByIdAsync(result.ProductItemID,
                    o => o.ProductImgs,
                    o => o.Product,
                    o => o.ProductItemAttributes);
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<ProductItem> UpdateAsync(int id, UpdateProductItemModel newProductItem)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(id,
                    o => o.ProductImgs);

                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {id} not found.");

                var product = await _unitOfWork.ProductRepository.GetByIdAsync(newProductItem.ProductID);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {newProductItem.ProductID} not found.");

                bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

                if (newProductItem.ProductID != 0 && newProductItem.ProductID != productItem.ProductID)
                {
                    productItem.ProductID = newProductItem.ProductID;
                }

                if (!IsInvalid(newProductItem.Name))
                {
                    productItem.Name = newProductItem.Name;
                }

                if (!IsInvalid(newProductItem.Description))
                {
                    productItem.Description = newProductItem.Description;
                }

                if (newProductItem.Quantity != default)
                {
                    productItem.Quantity = newProductItem.Quantity;
                }

                if (newProductItem.DisplayIndex != default)
                {
                    productItem.DisplayIndex = newProductItem.DisplayIndex;
                }

                if (newProductItem.Price != default)
                {
                    productItem.Price = newProductItem.Price;
                }

                if (newProductItem.RemoveImageIds?.Any() == true)
                {
                    var imagesToRemove = productItem.ProductImgs
                        .Where(img => newProductItem.RemoveImageIds.Contains(img.ProductImgID))
                        .ToList();

                    foreach (var img in imagesToRemove)
                    {
                        await _unitOfWork.ProductImgRepository.SoftDelete(img);
                    }
                }

                if (newProductItem.NewImageUrls?.Any() == true)
                {
                    var validImageUrls = newProductItem.NewImageUrls
                        .Where(url => !IsInvalid(url))
                        .ToList();

                    foreach (var imageUrl in validImageUrls)
                    {
                        var newImage = new ProductImg
                        {
                            ProductItemID = productItem.ProductItemID,
                            ImageUrl = imageUrl,
                            IsDeleted = false
                        };
                        await _unitOfWork.ProductImgRepository.AddAsync(newImage);
                    }
                }

                var result = await _unitOfWork.ProductItemRepository.Update(productItem);

                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to update product item.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return await _unitOfWork.ProductItemRepository.GetByIdAsync(id,
                    o => o.ProductImgs,
                    o => o.Product,
                    o => o.ProductItemAttributes);
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<ProductItem> DeleteAsync(int id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(id);
                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {id} not found.");

                var result = await _unitOfWork.ProductItemRepository.SoftDelete(productItem);
                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new InvalidOperationException("Failed to delete product item.");
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();
                return productItem;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }
    }
}