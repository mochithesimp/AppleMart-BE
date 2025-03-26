using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Models.ReviewModel;
using iPhoneBE.Service.Interfaces;
using System.Linq;
using System.Linq.Expressions;

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
                var user = await _unitOfWork.UserRepository.GetSingleByConditionAsynce(u => u.Id == review.UserID);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {review.UserID} not found.");

                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(review.ProductItemID);
                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {review.ProductItemID} not found.");

                var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(review.OrderDetailID);
                if (orderDetail == null)
                    throw new KeyNotFoundException($"OrderDetail with ID {review.OrderDetailID} not found.");

                if (!review.ProductRating.HasValue)
                    throw new ArgumentException("Product rating is required.");

                if (review.ShipperID != null)
                {
                    var shipper = await _unitOfWork.UserRepository.GetSingleByConditionAsynce(u => u.Id == review.ShipperID);
                    if (shipper == null)
                        throw new KeyNotFoundException($"Shipper with ID {review.ShipperID} not found.");

                    if (!review.ShipperRating.HasValue)
                        throw new ArgumentException("Shipper rating is required when a shipper is specified.");
                }
                else
                {
                    review.ShipperRating = null;
                    review.ShipperComment = null;
                }

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

        public async Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>> predicate)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                predicate,
                r => r.User,
                r => r.Shipper,
                r => r.ProductItem
            );

            return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
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

        public async Task<(double AverageRating, int TotalReviewers)> GetProductItemRatingStatisticsAsync(int productItemId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => r.ProductItemID == productItemId && !r.IsDeleted && r.ProductRating.HasValue
            );

            if (reviews == null || !reviews.Any())
                return (0, 0);

            var reviewsList = reviews.ToList();
            double averageRating = reviewsList.Average(r => r.ProductRating.Value);
            int totalReviewers = reviewsList.Count;

            return (Math.Round(averageRating, 1), totalReviewers);
        }

        public async Task<(double AverageRating, int TotalReviewers)> GetShipperRatingStatisticsAsync(string shipperId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => r.ShipperID == shipperId && !r.IsDeleted && r.ShipperRating.HasValue
            );

            if (reviews == null || !reviews.Any())
                return (0, 0);

            var reviewsList = reviews.ToList();
            double averageRating = reviewsList.Average(r => r.ShipperRating.Value);
            int totalReviewers = reviewsList.Count;

            return (Math.Round(averageRating, 1), totalReviewers);
        }

        public async Task<IEnumerable<Review>> GetAllProductReviewsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => !r.IsDeleted && r.ProductRating.HasValue,
                r => r.User,
                r => r.ProductItem,
                r => r.Shipper
            );

            return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
        }

        public async Task<IEnumerable<Review>> GetAllShipperReviewsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => !r.IsDeleted && r.ShipperRating.HasValue && r.ShipperID != null,
                r => r.User,
                r => r.Shipper,
                r => r.ProductItem
            );

            return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
        }

        public async Task<IDictionary<int, (double AverageRating, int TotalReviewers)>> GetAllProductRatingsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => !r.IsDeleted && r.ProductRating.HasValue,
                r => r.ProductItem
            );

            if (reviews == null || !reviews.Any())
                return new Dictionary<int, (double, int)>();

            var result = reviews
                .GroupBy(r => r.ProductItemID)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        AverageRating: Math.Round(g.Average(r => r.ProductRating.Value), 1),
                        TotalReviewers: g.Count()
                    )
                );

            return result;
        }

        public async Task<IDictionary<string, (double AverageRating, int TotalReviewers)>> GetAllShipperRatingsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
                r => !r.IsDeleted && r.ShipperRating.HasValue && r.ShipperID != null,
                r => r.Shipper
            );

            if (reviews == null || !reviews.Any())
                return new Dictionary<string, (double, int)>();

            var result = reviews
                .GroupBy(r => r.ShipperID)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        AverageRating: Math.Round(g.Average(r => r.ShipperRating.Value), 1),
                        TotalReviewers: g.Count()
                    )
                );

            return result;
        }
    }
}