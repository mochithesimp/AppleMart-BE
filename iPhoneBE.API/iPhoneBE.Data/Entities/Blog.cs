using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Author name is required.")]
        public string Author { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public DateTime UploadDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int View { get; set; }

        public int Like { get; set; }

        public bool IsActive { get; set; }

        public Product Product { get; set; }
        public virtual ICollection<UserBlogView> UserBlogViews { get; set; }
        public virtual ICollection<BlogImage> BlogImages { get; set; }
    }
}
