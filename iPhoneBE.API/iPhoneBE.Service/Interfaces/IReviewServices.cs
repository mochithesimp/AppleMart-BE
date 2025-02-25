using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.ReviewModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IReviewServices
    {
        Task<Review> AddAsync(Review review);
        Task<IEnumerable<Review>> GetByProductItemIdAsync(int productItemId);
        Task<IEnumerable<Review>> GetByUserIdAsync(string userId);
    }
}