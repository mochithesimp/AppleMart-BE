using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Data.Models.OrderModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IOrderServices
    {
        Task<Order> AddAsync(OrderModel model);
        Task<Order> GetByIdAsync(int id);
        Task<object> GetOrdersAsync(Guid? userId, string? status, TimeModel model, string userRole, Guid currentUserId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, User user, string? shipperId = null);
    }
}