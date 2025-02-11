using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;

namespace iPhoneBE.Data.Model
{
    public class Voucher
    {
        [Key]
        public int VoucherID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string DiscountType { get; set; }

        public int DiscountValue { get; set; }

        public double MinimumTotal { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ExpiredDate { get; set; }

        public bool IsActive { get; set; }


        [ForeignKey("ProductItem")]
        public int ProductItemID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public virtual ProductItem ProductItem { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
