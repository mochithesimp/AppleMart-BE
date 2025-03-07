
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Service.Extentions;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace iPhoneBE.Service.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminServices(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetTotalUserAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<object> GetTotalRevenueAsync(TimeModel model)
        {
            var query = _unitOfWork.OrderRepository.GetAllQueryable()
                .FilterByYear(model.Year)
                .FilterByMonth(model.Month, model.Year)
                .FilterByDay(model.Day, model.Month, model.Year);

            double totalRevenue = await query.SumAsync(o => o.Total);

            return new
            {
                IsFiltered = model.Year.HasValue || model.Month.HasValue || model.Day.HasValue,
                Year = model.Year,
                Month = model.Month,
                Day = model.Day,
                TotalRevenue = totalRevenue
            };
        }

    }
}
