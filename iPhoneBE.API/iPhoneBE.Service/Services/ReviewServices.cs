using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Models.ReviewModel;
using iPhoneBE.Service.Interfaces;

namespace iPhoneBE.Service.Services
{
    public class ReviewServices : IReviewServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Review> AddAsync(Review review)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //var user = await _unitOfWork.UserRepository.GetByIdAsync(review.UserID);
                //if (user == null)
                //    throw new KeyNotFoundException($"User with ID {review.UserID} not found.");

                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(review.ProductItemID);
                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {review.ProductItemID} not found.");

                //var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(review.OrderDetailID);
                //if (orderDetail == null)
                //    throw new KeyNotFoundException($"OrderDetail with ID {review.OrderDetailID} not found.");

                //if (review.ShipperID != null)
                //{
                //    var shipper = await _unitOfWork.UserRepository.GetByIdAsync(review.ShipperID);
                //    if (shipper == null)
                //        throw new KeyNotFoundException($"Shipper with ID {review.ShipperID} not found.");
                //}

                review.Date = DateTime.Now;
                var result = await _unitOfWork.ReviewRepository.AddAsync(review);

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

        public async Task<IEnumerable<Review>> GetByProductItemIdAsync(int productItemId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => r.ProductItemID == productItemId && !r.IsDeleted,
                r => r.User,
                r => r.Shipper
            );

            return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => r.UserID == userId && !r.IsDeleted,
                r => r.ProductItem,
                r => r.Shipper
            );

            return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
        }
    }
}