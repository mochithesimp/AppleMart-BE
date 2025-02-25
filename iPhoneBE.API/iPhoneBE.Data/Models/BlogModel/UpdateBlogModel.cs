using System.ComponentModel.DataAnnotations;
using iPhoneBE.Data.Models.BlogImageModel;

namespace iPhoneBE.Data.Models.BlogModel
{
    public class UpdateBlogModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        public string Author { get; set; }
        public int ProductId { get; set; }

        public List<CreateBlogImageModel> BlogImages { get; set; } = new List<CreateBlogImageModel>();
    }
}