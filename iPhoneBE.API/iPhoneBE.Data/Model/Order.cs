using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Model
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; } 

        [ForeignKey("User")]
        public int UserID { get; set; } 

        public DateTime OrderDate { get; set; } 

        [MaxLength(500)]
        public string Address { get; set; } 

        [MaxLength(500)]
        public string PaymentMethod { get; set; } 

        [ForeignKey("ShippingMethod")]
        public int ShippingMethodID { get; set; } 

        public decimal Total { get; set; } 

        [MaxLength(250)]
        public string OrderStatus { get; set; } 

        [ForeignKey("Voucher")]
        public int? VoucherID { get; set; } 

        // Navigation properties
        public User User { get; set; } // Liên kết với User
        public ShippingMethod ShippingMethod { get; set; }
        public Voucher Voucher { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
