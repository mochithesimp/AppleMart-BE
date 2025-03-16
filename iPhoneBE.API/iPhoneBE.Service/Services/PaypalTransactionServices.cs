using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Models.PaypalModel;
using iPhoneBE.Service.Interfaces;

namespace iPhoneBE.Service.Services
{
    public class PaypalTransactionServices : IPaypalTransactionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaypalTransactionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaypalTransaction> CreateTransactionAsync(CreatePaypalTransactionModel model)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transaction = _mapper.Map<PaypalTransaction>(model);
                var result = await _unitOfWork.PaypalTransactionRepository.AddAsync(transaction);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return result;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PaypalTransaction> GetTransactionByIdAsync(int id)
        {
            var transaction = await _unitOfWork.PaypalTransactionRepository.GetByIdAsync(id);
            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {id} not found.");

            return transaction;
        }

        public async Task<PaypalTransaction> GetTransactionByPaypalPaymentIdAsync(string paypalPaymentId)
        {
            var transaction = await _unitOfWork.PaypalTransactionRepository.GetSingleByConditionAsynce(
                x => x.PaypalPaymentId == paypalPaymentId && !x.IsDeleted
            );

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with PayPal Payment ID {paypalPaymentId} not found.");

            return transaction;
        }

        public async Task<IEnumerable<PaypalTransaction>> GetTransactionsByOrderIdAsync(int orderId)
        {
            var transactions = await _unitOfWork.PaypalTransactionRepository.GetAllAsync(
                x => x.OrderId == orderId && !x.IsDeleted
            );

            return transactions;
        }

        public async Task<PaypalTransaction> UpdateTransactionStatusAsync(int id, string status)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transaction = await _unitOfWork.PaypalTransactionRepository.GetByIdAsync(id);
                if (transaction == null)
                    throw new KeyNotFoundException($"Transaction with ID {id} not found.");

                transaction.Status = status;
                await _unitOfWork.PaypalTransactionRepository.Update(transaction);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return transaction;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}