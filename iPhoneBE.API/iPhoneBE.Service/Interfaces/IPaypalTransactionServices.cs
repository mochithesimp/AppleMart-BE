using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.PaypalModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IPaypalTransactionServices
    {
        Task<PaypalTransaction> CreateTransactionAsync(CreatePaypalTransactionModel model);
        Task<PaypalTransaction> GetTransactionByIdAsync(int id);
        Task<PaypalTransaction> GetTransactionByPaypalPaymentIdAsync(string paypalPaymentId);
        Task<IEnumerable<PaypalTransaction>> GetTransactionsByOrderIdAsync(int orderId);
        Task<PaypalTransaction> UpdateTransactionStatusAsync(int id, string status);
    }
}