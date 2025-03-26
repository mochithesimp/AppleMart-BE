using System.ComponentModel.DataAnnotations;
using iPhoneBE.Data.Models.BlogImageModel;
using System.Text.Json.Serialization;

namespace iPhoneBE.Data.Models.BlogModel
{
    public class CreateBlogModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Author name is required.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; private set; } = false;

        public List<CreateBlogImageModel> BlogImages { get; set; } = new List<CreateBlogImageModel>();
    }
}