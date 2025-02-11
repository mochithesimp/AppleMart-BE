using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iPhoneBE.Data.Entities;

namespace iPhoneBE.Data.Model
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<ProductItem> ProductItems { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
