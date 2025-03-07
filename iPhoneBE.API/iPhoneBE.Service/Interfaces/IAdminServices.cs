
using iPhoneBE.Data.Models.AdminModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IAdminServices
    {
        Task<object> GetTotalRevenueAsync(TimeModel model);
        Task<int> GetTotalUserAsync();
    }
}