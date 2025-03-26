using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.ReviewModel;
using System.Linq.Expressions;

namespace iPhoneBE.Service.Interfaces
{
    public interface IReviewServices
    {
        Task<Review> AddAsync(Review review);
        Task<IEnumerable<Review>> GetByProductItemIdAsync(int productItemId);
        Task<IEnumerable<Review>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>> predicate);
        Task<(double AverageRating, int TotalReviewers)> GetProductItemRatingStatisticsAsync(int productItemId);
        Task<(double AverageRating, int TotalReviewers)> GetShipperRatingStatisticsAsync(string shipperId);
        Task<IEnumerable<Review>> GetAllProductReviewsAsync();
        Task<IEnumerable<Review>> GetAllShipperReviewsAsync();
        Task<IDictionary<int, (double AverageRating, int TotalReviewers)>> GetAllProductRatingsAsync();
        Task<IDictionary<string, (double AverageRating, int TotalReviewers)>> GetAllShipperRatingsAsync();
    }
}