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
    public class Review
    {
        public int ReviewID { get; set; }

        [ForeignKey("User")]
        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

        [ForeignKey("OrderDetail")]
        [Required(ErrorMessage = "Order Detail ID is required.")]
        public int OrderDetailID { get; set; }

        [ForeignKey("ProductItem")]
        [Required(ErrorMessage = "Product Item ID is required.")]
        public int ProductItemID { get; set; }

        [ForeignKey("Shipper")]
        public int? ShipperID { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }
        public User User { get; set; }
        public User Shipper { get; set; }
        public OrderDetail OrderDetail { get; set; }
        public ProductItem ProductItem { get; set; }
    }
}
