using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.ReviewModel
{
    public class CreateReviewModel
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Order Detail ID is required.")]
        public int OrderDetailID { get; set; }

        [Required(ErrorMessage = "Product Item ID is required.")]
        public int ProductItemID { get; set; }

        public string? ShipperID { get; set; }

        [Required(ErrorMessage = "Product rating is required.")]
        [Range(1, 5, ErrorMessage = "Product rating must be between 1 and 5.")]
        public int ProductRating { get; set; }

        [Range(1, 5, ErrorMessage = "Shipper rating must be between 1 and 5.")]
        public int? ShipperRating { get; set; }

        [MaxLength(1000, ErrorMessage = "Product comment cannot exceed 1000 characters.")]
        public string? ProductComment { get; set; }

        [MaxLength(1000, ErrorMessage = "Shipper comment cannot exceed 1000 characters.")]
        public string? ShipperComment { get; set; }
    }
}