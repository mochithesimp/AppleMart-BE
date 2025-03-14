using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.ProductImgModel
{
    public class UpdateProductImgModel
    {
        [Required(ErrorMessage = "Image URL is required.")]
        [MaxLength(1000, ErrorMessage = "Image URL cannot exceed 1000 characters.")]
        public string ImageUrl { get; set; }
        public int ProductImgID { get; set; }
    }
}