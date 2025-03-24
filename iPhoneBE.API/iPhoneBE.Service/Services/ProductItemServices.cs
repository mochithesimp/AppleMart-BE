using AutoMapper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Service.Extensions;
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

        public async Task<PagedResult<ProductItem>> GetAllAsync(ProductItemFilterModel filter)
        {
            var query = _unitOfWork.ProductItemRepository.GetAllQueryable()
                .ApplyBaseQuery()
                .FilterByCategoryName(filter.CategoryName)
                .FilterBySearchTerm(filter.SearchTerm)
                .FilterByPriceRange(filter.MinPrice, filter.MaxPrice)
                .FilterByColors(filter.Colors)
                .FilterByRAM(filter.RAMSizes)
                .FilterByROM(filter.ROMSizes)
                .ApplySorting(filter.PriceSort);

            return await query.ToPagedResultAsync(filter);
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
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productItem.ProductID);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {productItem.ProductID} not found.");

                var images = productItem.ProductImgs?.ToList() ?? new List<ProductImg>();
                productItem.ProductImgs = new List<ProductImg>();

                var result = await _unitOfWork.ProductItemRepository.AddAsync(productItem);
                await _unitOfWork.SaveChangesAsync();

                if (images.Any())
                {
                    foreach (var img in images)
                    {
                        img.ProductItemID = result.ProductItemID;
                        await _unitOfWork.ProductImgRepository.AddAsync(img);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                return await _unitOfWork.ProductItemRepository.GetByIdAsync(result.ProductItemID,
                    o => o.ProductImgs,
                    o => o.Product,
                    o => o.ProductItemAttributes);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ProductItem> UpdateAsync(int id, UpdateProductItemModel newProductItem)
        {
            await _unitOfWork.BeginTransactionAsync();
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

                if (newProductItem.UpdatedProductImgs?.Any() == true)
                {
                    foreach (var updatedImg in newProductItem.UpdatedProductImgs)
                    {
                        var existingImg = productItem.ProductImgs
                            .FirstOrDefault(img => img.ProductImgID == updatedImg.ProductImgID);

                        if (existingImg != null && !IsInvalid(updatedImg.ImageUrl))
                        {
                            existingImg.ImageUrl = updatedImg.ImageUrl;
                            await _unitOfWork.ProductImgRepository.Update(existingImg);
                        }
                    }
                }

                //if (newProductItem.NewImageUrls?.Any() == true)
                //{
                //    var validImageUrls = newProductItem.NewImageUrls
                //        .Where(url => !IsInvalid(url))
                //        .ToList();

                //    foreach (var imageUrl in validImageUrls)
                //    {
                //        var newImage = new ProductImg
                //        {
                //            ProductItemID = productItem.ProductItemID,
                //            ImageUrl = imageUrl,
                //            IsDeleted = false
                //        };
                //        await _unitOfWork.ProductImgRepository.AddAsync(newImage);
                //    }
                //}

                var result = await _unitOfWork.ProductItemRepository.Update(productItem);

                if (!result)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new InvalidOperationException("Failed to update product item.");
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await _unitOfWork.ProductItemRepository.GetByIdAsync(id,
                    o => o.ProductImgs,
                    o => o.Product,
                    o => o.ProductItemAttributes);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ProductItem> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(id);
                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {id} not found.");

                var result = await _unitOfWork.ProductItemRepository.SoftDelete(productItem);
                if (!result)
                {
                    throw new InvalidOperationException("Failed to delete product item.");
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return productItem;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}