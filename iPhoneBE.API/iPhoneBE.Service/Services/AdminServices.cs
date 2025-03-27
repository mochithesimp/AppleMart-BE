using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.ViewModels.ProductItemVM;
using iPhoneBE.Data.ViewModels.UserVM;
using iPhoneBE.Service.Extensions;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace iPhoneBE.Service.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserServices _userServices;

        public AdminServices(UserManager<User> userManager, IUnitOfWork unitOfWork, IUserServices userServices)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userServices = userServices;
        }

        public async Task<int> GetTotalUserAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<object> GetTotalRevenueAsync(TimeModel model)
        {
            var query = _unitOfWork.OrderRepository.GetAllQueryable()
                .FilterByYear(model.Year)
                .FilterByQuarter(model.Quarter, model.Year)
                .FilterByMonth(model.Month, model.Year)
                .FilterByDay(model.Day, model.Month, model.Year);


            double totalRevenue = await query.SumAsync(o => o.Total);

            return new
            {
                IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue,
                Year = model.Year,
                Quarter = model.Quarter,
                Month = model.Month,
                Day = model.Day,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<object> GetTopSellingProductItemsAsync(TimeModel model, int? topN)
        {
            var query = _unitOfWork.OrderRepository.GetAllQueryable()
                //.Where(o => o.OrderStatus == "Completed")
                .FilterByYear(model.Year)
                .FilterByQuarter(model.Quarter, model.Year) // Thêm bộ lọc theo quý
                .FilterByMonth(model.Month, model.Year)
                .FilterByDay(model.Day, model.Month, model.Year);

            var topProducts = await query
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.ProductItemID)
                .Select(g => new
                {
                    ProductItemId = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(topN ?? int.MaxValue) // topN or all
                .ToListAsync();

            var productItemIds = topProducts.Select(p => p.ProductItemId).ToList();

            var products = await _unitOfWork.ProductItemRepository
                .GetAllQueryable()
                .Where(p => productItemIds.Contains(p.ProductItemID))
                .ToListAsync();

            //last infomation
            var result = products.Select(p => new ProductItemSalesViewModel
            {
                ProductItemId = p.ProductItemID,
                Name = p.Name,
                Price = p.Price,
                TotalSold = topProducts.FirstOrDefault(tp => tp.ProductItemId == p.ProductItemID)?.TotalSold ?? 0
            })
            .OrderByDescending(p => p.TotalSold) // Sắp xếp lại từ cao xuống thấp
            .ToList();

            return new
            {
                IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue,
                Year = model.Year,
                Quarter = model.Quarter,
                Month = model.Month,
                Day = model.Day,
                TopProductItems = result
            };
        }

        public async Task<object> GetTopCustomersAsync(TimeModel model, int? topN)
        {
            var query = _unitOfWork.OrderRepository.GetAllQueryable()
                //.Where(o => o.OrderStatus == "Completed") // Chỉ lấy đơn hàng đã hoàn thành
                .FilterByYear(model.Year)
                .FilterByQuarter(model.Quarter, model.Year)
                .FilterByMonth(model.Month, model.Year)
                .FilterByDay(model.Day, model.Month, model.Year);

            var topCustomers = await query
                .GroupBy(o => o.UserID)
                .Select(g => new
                {
                    UserID = g.Key,
                    TotalSpent = g.Sum(o => o.Total), // Tổng số tiền đã chi tiêu
                    OrderCount = g.Count() // Số lượng đơn hàng đã đặt
                })
                .OrderByDescending(g => g.TotalSpent)
                .Take(topN ?? int.MaxValue) // Nếu không truyền topN, lấy tất cả
                .ToListAsync();

            return new
            {
                IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue,
                Year = model.Year,
                Quarter = model.Quarter,
                Month = model.Month,
                Day = model.Day,
                TopCustomers = topCustomers
            };
        }

        public async Task<object> GetTotalProductItemsAsync(CategoryProductFilterModel filter)
        {
            var total = 0;

            var totalQuantityData = await _unitOfWork.ProductItemRepository.GetAllQueryable()
                .ApplyBaseQuery()
                .GetTotalQuantityByCategoryAndProduct(filter.CategoryId, filter.ProductId)
                .ToListAsync();

            if (totalQuantityData != null)
            {
                total = totalQuantityData.Sum(t => t.TotalQuantity);
            }

            return new
            {
                total = total,
                totalQuantityData = totalQuantityData
            };
        }

        public async Task<IEnumerable<ShipperViewModel>> GetAllShippersWithPendingOrdersAsync()
        {
            return await _userServices.GetAllShippersWithPendingOrdersAsync();
        }
    }
}
