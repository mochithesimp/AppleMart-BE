
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.ViewModels.ProductItemVM;

namespace iPhoneBE.Service.Interfaces
{
    public interface IAdminServices
    {
        Task<object> GetTopCustomersAsync(TimeModel model, int? topN);
        Task<object> GetTopSellingProductItemsAsync(TimeModel model, int? topN);
        Task<object> GetTotalProductItemsAsync(CategoryProductFilterModel filter);
        Task<object> GetTotalRevenueAsync(TimeModel model);
        Task<int> GetTotalUserAsync();
    }
}