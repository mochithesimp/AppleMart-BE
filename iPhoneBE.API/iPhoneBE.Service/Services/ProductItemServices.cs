using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Helper;
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

        public async Task<List<ProductItem>> GetAllWithoutFilter()
        {
            var productItems = await _unitOfWork.ProductItemRepository.GetAllQueryable()
                .Where(r => r.IsDeleted == false)
                .Select(pi => new ProductItem
                {
                    ProductItemID = pi.ProductItemID,
                    ProductID = pi.ProductID,
                    Name = pi.Name,
                    Description = pi.Description,
                    Quantity = pi.Quantity,
                    DisplayIndex = pi.DisplayIndex,
                    Price = pi.Price,
                    IsDeleted = pi.IsDeleted,
                    Product = new Product
                    {
                        ProductID = pi.Product.ProductID,
                        CategoryID = pi.Product.CategoryID,
                        Name = pi.Product.Name,
                        Description = pi.Product.Description,
                        IsDeleted = pi.Product.IsDeleted,
                        DisplayIndex = pi.Product.DisplayIndex,
                        Category = pi.Product.Category
                    },
                    ProductImgs = pi.ProductImgs.Where(img => !img.IsDeleted)
                        .Select(img => new ProductImg
                        {
                            ProductImgID = img.ProductImgID,
                            ProductItemID = img.ProductItemID,
                            ImageUrl = img.ImageUrl,
                            IsDeleted = img.IsDeleted
                        }).ToList(),
                    ProductItemAttributes = pi.ProductItemAttributes
                        .Select(attr => new ProductItemAttribute
                        {
                            ProductItemAttributeID = attr.ProductItemAttributeID,
                            ProductItemID = attr.ProductItemID,
                            AttributeID = attr.AttributeID,
                            Value = attr.Value,
                            Attribute = new iPhoneBE.Data.Entities.Attribute
                            {
                                AttributeID = attr.Attribute.AttributeID,
                                AttributeName = attr.Attribute.AttributeName
                            }
                        }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return productItems;
        }

        public async Task<PagedResult<ProductItem>> GetAllAsync(ProductItemFilterModel filter)
        {
            var query = _unitOfWork.ProductItemRepository.GetAllQueryable()
                .ApplyBaseQuery()
                .FilterByCategory(filter.CategoryId)
                //.FilterByCategoryName(filter.CategoryName)
                .FilterBySearchTerm(filter.SearchTerm)
                .FilterByPriceRange(filter.MinPrice, filter.MaxPrice)
                .FilterByColors(filter.Colors)
                .FilterByRAM(filter.RAMSizes)
                .FilterByROM(filter.ROMSizes)
                .FilterByCPU(filter.CPUs)
                .FilterByStorage(filter.Storages)
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

        public async Task<object> GetTotalSoldForProductItemAsync(int productItemId)
        {
            // Get the product item to verify it exists
            var productItem = await _unitOfWork.ProductItemRepository
                .GetAllQueryable()
                .Where(pi => pi.ProductItemID == productItemId && !pi.IsDeleted)
                .FirstOrDefaultAsync();

            if (productItem == null)
            {
                throw new KeyNotFoundException($"ProductItem with ID {productItemId} not found or has been deleted.");
            }

            // Query to count total sold quantity in completed orders only
            var totalSold = await _unitOfWork.OrderRepository
                .GetAllQueryable()
                .Where(o => o.OrderStatus == OrderStatusHelper.Completed) // Only count completed orders
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.ProductItemID == productItemId)
                .SumAsync(od => od.Quantity);

            return new
            {
                ProductItemId = productItemId,
                ProductItemName = productItem.Name,
                TotalSold = totalSold
            };
        }
    }
}