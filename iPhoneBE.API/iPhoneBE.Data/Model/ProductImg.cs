using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Model
{
    public class ProductImg
    {
        {
        [Key]
        public int ProductImgID { get; set; }

        [ForeignKey("ProductItem")]
        public int ProductItemID { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation property for ProductItem
        public virtual ProductItem ProductItem { get; set; }
    }
}
}
