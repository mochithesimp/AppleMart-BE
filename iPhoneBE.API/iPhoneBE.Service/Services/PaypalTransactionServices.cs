using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Models.PaypalModel;
using iPhoneBE.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Services
{
    public class PaypalTransactionServices : IPaypalTransactionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PayPalService _payPalService;

        public PaypalTransactionServices(IUnitOfWork unitOfWork, IMapper mapper, PayPalService payPalService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payPalService = payPalService;
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

        public async Task<PaypalTransaction> ProcessRefundAsync(int transactionId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transaction = await _unitOfWork.PaypalTransactionRepository.GetByIdAsync(transactionId);
                if (transaction == null)
                    throw new KeyNotFoundException($"Transaction with ID {transactionId} not found.");

                Console.WriteLine($"Processing refund for transaction: ID={transactionId}, Status={transaction.Status}, Amount={transaction.Amount}, PayPalID={transaction.PaypalPaymentId}");

                if (transaction.Status.ToUpper() != "COMPLETED")
                    throw new InvalidOperationException($"Transaction status must be 'COMPLETED' to be refunded. Current status: {transaction.Status}");

                if (string.IsNullOrEmpty(transaction.PaypalPaymentId))
                    throw new InvalidOperationException("PayPal Payment ID is missing. Cannot process refund.");

                try
                {
                    var refund = await _payPalService.ProcessRefundAsync(
                        transaction.PaypalPaymentId,
                        (decimal)transaction.Amount,
                        transaction.Currency
                    );

                    if (refund == null)
                        throw new Exception("PayPal refund returned null response");

                    transaction.Status = "REFUNDED";
                    await _unitOfWork.PaypalTransactionRepository.Update(transaction);

                    var refundTransaction = new PaypalTransaction
                    {
                        OrderId = transaction.OrderId,
                        PaypalPaymentId = refund.id,
                        Status = "REFUNDED",
                        Amount = -transaction.Amount, 
                        Currency = transaction.Currency,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await _unitOfWork.PaypalTransactionRepository.AddAsync(refundTransaction);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return refundTransaction;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"PayPal refund error: {ex.Message}");
                    throw new Exception($"PayPal refund failed: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Console.WriteLine($"Refund process error: {ex.Message}");
                throw;
            }
        }
    }
}