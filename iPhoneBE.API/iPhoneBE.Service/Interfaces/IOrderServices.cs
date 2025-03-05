using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.OrderModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IOrderServices
    {
        Task<Order> AddAsync(OrderModel model);
        Task<Order> GetByIdAsync(int id);
    }
}