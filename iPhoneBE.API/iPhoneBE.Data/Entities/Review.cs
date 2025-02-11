using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class Review
    {
        public int ReviewID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("OrderDetail")]
        public int OrderDetailID { get; set; }

        [ForeignKey("ProductItem")]
        public int ProductItemID { get; set; }

        [ForeignKey("Shipper")]
        public int? ShipperID { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public User User { get; set; }
        public User Shipper { get; set; }
        public OrderDetail OrderDetail { get; set; }
        public ProductItem ProductItem { get; set; }
    }
}
