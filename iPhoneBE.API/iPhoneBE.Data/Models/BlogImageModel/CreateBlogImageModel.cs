using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.BlogImageModel
{
    public class CreateBlogImageModel
    {
        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl { get; set; }
    }
}