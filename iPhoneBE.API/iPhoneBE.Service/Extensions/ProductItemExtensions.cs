using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductItemModel;
using Microsoft.EntityFrameworkCore;

namespace iPhoneBE.Service.Extensions
{
    public static class ProductItemExtensions
    {
        public static IQueryable<ProductItem> ApplyBaseQuery(this IQueryable<ProductItem> query)
        {
            return query
                .Include(o => o.ProductImgs.Where(img => !img.IsDeleted))
                .Include(o => o.Product)
                .Include(o => o.ProductItemAttributes)
                    .ThenInclude(pia => pia.Attribute)
                .Where(o => !o.IsDeleted);
        }

        public static IQueryable<ProductItem> FilterByCategory(this IQueryable<ProductItem> query, int? categoryId)
        {
            return categoryId.HasValue
                ? query.Where(p => p.Product.CategoryID == categoryId.Value)
                : query;
        }

        public static IQueryable<ProductItem> FilterByProduct(this IQueryable<ProductItem> query, int? productId)
        {
            return productId.HasValue
                ? query.Where(p => p.ProductID == productId.Value)
                : query;
        }

        public static IQueryable<ProductItem> FilterBySearchTerm(this IQueryable<ProductItem> query, string? searchTerm)
        {
            return !string.IsNullOrWhiteSpace(searchTerm)
                ? query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
                : query;
        }

        public static IQueryable<ProductItemSummaryModel> GetTotalQuantityByCategoryAndProduct(
            this IQueryable<ProductItem> query, int? categoryId = null, int? productId = null)
        {
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.Product.CategoryID == categoryId.Value);
            }

            if (productId.HasValue)
            {
                query = query.Where(p => p.ProductID == productId.Value);
            }

            return query
                .GroupBy(p => new { p.Product.CategoryID, p.ProductID })
                .Select(g => new ProductItemSummaryModel
                {
                    CategoryId = g.Key.CategoryID,
                    ProductId = g.Key.ProductID,
                    TotalQuantity = g.Sum(p => p.Quantity)
                });
        }


        public static IQueryable<ProductItem> FilterByPriceRange(this IQueryable<ProductItem> query, decimal? minPrice, decimal? maxPrice)
        {
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= (double)minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= (double)maxPrice.Value);
            }
            return query;
        }

        public static IQueryable<ProductItem> FilterByColors(this IQueryable<ProductItem> query, List<string>? colors)
        {
            return colors?.Any() == true
                ? query.Where(p => p.ProductItemAttributes
                    .Any(pia => pia.Attribute.AttributeName == "Color" &&
                               colors.Contains(pia.Value)))
                : query;
        }

        public static IQueryable<ProductItem> FilterByRAM(this IQueryable<ProductItem> query, List<string>? ramSizes)
        {
            return ramSizes?.Any() == true
                ? query.Where(p => p.ProductItemAttributes
                    .Any(pia => pia.Attribute.AttributeName == "RAM" &&
                               ramSizes.Contains(pia.Value)))
                : query;
        }

        public static IQueryable<ProductItem> FilterByROM(this IQueryable<ProductItem> query, List<string>? romSizes)
        {
            return romSizes?.Any() == true
                ? query.Where(p => p.ProductItemAttributes
                    .Any(pia => pia.Attribute.AttributeName == "ROM" &&
                               romSizes.Contains(pia.Value)))
                : query;
        }

        public static IQueryable<ProductItem> FilterByCPU(this IQueryable<ProductItem> query, List<string>? cpus)
        {
            return cpus?.Any() == true
                ? query.Where(p => p.ProductItemAttributes
                    .Any(pia => pia.Attribute.AttributeName == "CPU" &&
                               cpus.Contains(pia.Value)))
                : query;
        }

        public static IQueryable<ProductItem> FilterByStorage(this IQueryable<ProductItem> query, List<string>? storages)
        {
            return storages?.Any() == true
                ? query.Where(p => p.ProductItemAttributes
                    .Any(pia => pia.Attribute.AttributeName == "Storage" &&
                               storages.Contains(pia.Value)))
                : query;
        }

        public static IQueryable<ProductItem> ApplySorting(this IQueryable<ProductItem> query, string? priceSort)
        {
            if (!string.IsNullOrEmpty(priceSort))
            {
                return priceSort.ToLower() == "lowtohigh"
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price);
            }
            return query.OrderBy(p => p.DisplayIndex);
        }

        public static async Task<PagedResult<ProductItem>> ToPagedResultAsync(
            this IQueryable<ProductItem> query,
            ProductItemFilterModel filter)
        {
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize);

            filter.ValidatePageNumber(totalPages);

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<ProductItem>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages
            };
        }

        public static IQueryable<ProductItem> FilterByCategoryName(this IQueryable<ProductItem> query, string? categoryName)
        {
            return !string.IsNullOrWhiteSpace(categoryName)
                ? query.Where(p => p.Product.Category.Name.ToLower() == categoryName.ToLower())
                : query;
        }
    }
}