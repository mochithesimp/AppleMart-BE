using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.ProductImgModel
{
    public class CreateProductImgModel
    {
        [Required(ErrorMessage = "Image URL is required.")]
        [MaxLength(1000, ErrorMessage = "Image URL cannot exceed 1000 characters.")]
        public List<string> ImageUrl { get; set; } = new List<string>();
        public int ProductItemID { get; set; }

        //public string ImageUrl { get; set; }
    }
}