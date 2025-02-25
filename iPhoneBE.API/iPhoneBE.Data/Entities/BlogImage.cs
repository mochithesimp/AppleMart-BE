using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class BlogImage : IBaseEntity
    {
        [Key]
        public int BlogImageID { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("BlogId")]
        public int BlogId { get; set; }

        public Blog Blog { get; set; }
    }
}
