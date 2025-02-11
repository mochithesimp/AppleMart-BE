using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class UserBlogView
    {
        [Key]
        public int UserBlogViewId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [ForeignKey("BlogId")]
        public int BlogId { get; set; }
        public int Like { get; set; }

        public Blog Blog { get; set; }
        public User User { get; set; }
    }
}
