using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.Models.ReviewModel;
using iPhoneBE.Data.ViewModels.ReviewVM;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewServices _reviewServices;
        private readonly IProductItemServices _productItemServices;
        private readonly IUserServices _userServices;
        private readonly IMapper _mapper;

        public ReviewController(IReviewServices reviewServices, IProductItemServices productItemServices,
            IUserServices userServices, IMapper mapper)
        {
            _reviewServices = reviewServices;
            _productItemServices = productItemServices;
            _userServices = userServices;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ReviewViewModel>> Add([FromBody] CreateReviewModel createReview)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var review = _mapper.Map<Review>(createReview);

            if (review == null)
                return BadRequest("Invalid review data.");

            review = await _reviewServices.AddAsync(review);

            return Ok(_mapper.Map<ReviewViewModel>(review));
        }

        [HttpGet("product/{productItemId}/details")]
        public async Task<ActionResult<ProductRatingViewModel>> GetProductDetails(int productItemId)
        {
            var productItem = await _productItemServices.GetByIdAsync(productItemId);
            if (productItem == null)
                return NotFound($"Product item with ID {productItemId} not found.");

            var reviews = await _reviewServices.GetByProductItemIdAsync(productItemId);

            var (averageRating, totalReviewers) = await _reviewServices.GetProductItemRatingStatisticsAsync(productItemId);

            return Ok(new ProductRatingViewModel
            {
                ProductItemID = productItemId,
                ProductName = productItem.Name,
                AverageRating = averageRating,
                TotalReviewers = totalReviewers,
                Reviews = _mapper.Map<List<ReviewViewModel>>(reviews)
            });
        }

        [HttpGet("shipper/{shipperId}/details")]
        public async Task<ActionResult<ShipperRatingViewModel>> GetShipperDetails(string shipperId)
        {
            var shipper = await _userServices.GetByIdAsync(shipperId);
            if (shipper == null)
                return NotFound($"Shipper with ID {shipperId} not found.");

            var reviews = await _reviewServices.GetAllAsync(r => r.ShipperID == shipperId && !r.IsDeleted);

            var (averageRating, totalReviewers) = await _reviewServices.GetShipperRatingStatisticsAsync(shipperId);

            return Ok(new ShipperRatingViewModel
            {
                ShipperID = shipperId,
                ShipperName = shipper.Name,
                AverageRating = averageRating,
                TotalReviewers = totalReviewers,
                Reviews = _mapper.Map<List<ReviewViewModel>>(reviews)
            });
        }

        [HttpGet("products/summary")]
        public async Task<ActionResult<IEnumerable<ProductRatingViewModel>>> GetAllProductRatings()
        {
            var filter = new ProductItemFilterModel
            {
                PageNumber = 1,
                PageSize = 1000
            };

            var productItemsResult = await _productItemServices.GetAllAsync(filter);
            var productItems = productItemsResult.Items;

            var allRatings = await _reviewServices.GetAllProductRatingsAsync();

            var result = new List<ProductRatingViewModel>();

            foreach (var product in productItems)
            {
                double avgRating = 0;
                int totalReviewers = 0;

                if (allRatings.TryGetValue(product.ProductItemID, out var stats))
                {
                    avgRating = stats.AverageRating;
                    totalReviewers = stats.TotalReviewers;
                }

                result.Add(new ProductRatingViewModel
                {
                    ProductItemID = product.ProductItemID,
                    ProductName = product.Name,
                    AverageRating = avgRating,
                    TotalReviewers = totalReviewers
                });
            }

            return Ok(result);
        }

        [HttpGet("shippers/summary")]
        public async Task<ActionResult<IEnumerable<ShipperRatingViewModel>>> GetAllShipperRatings()
        {
            var shippers = await _userServices.GetShippersAsync();

            var allRatings = await _reviewServices.GetAllShipperRatingsAsync();

            var result = new List<ShipperRatingViewModel>();

            foreach (var shipper in shippers)
            {
                double avgRating = 0;
                int totalReviewers = 0;

                if (allRatings.TryGetValue(shipper.Id, out var stats))
                {
                    avgRating = stats.AverageRating;
                    totalReviewers = stats.TotalReviewers;
                }

                result.Add(new ShipperRatingViewModel
                {
                    ShipperID = shipper.Id,
                    ShipperName = shipper.Name,
                    AverageRating = avgRating,
                    TotalReviewers = totalReviewers
                });
            }

            return Ok(result);
        }
    }
}