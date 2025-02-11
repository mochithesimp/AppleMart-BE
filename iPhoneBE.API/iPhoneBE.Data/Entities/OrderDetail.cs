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
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [ForeignKey("ProductItem")]
        public int ProductItemID { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }

        public ProductItem ProductItem { get; set; }
        public Order Order { get; set; }
        public virtual ICollection<Review> Reviews { get; set;}
    }
}
