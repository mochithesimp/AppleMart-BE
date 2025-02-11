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
    public class ProductItem
    {
        [Key]
        public int ProductItemID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string Color { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        [MaxLength(1000)]
        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Product Product { get; set; }

        public virtual ICollection<ProductImg> ProductImgs { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
