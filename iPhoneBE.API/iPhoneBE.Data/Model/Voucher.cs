using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Model
{
    public class Voucher
    {
        [Key]
        public int PKVoucherID { get; set; }

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

        // Navigation property for ProductItem
        public virtual ProductItem ProductItem { get; set; }
    }
}
