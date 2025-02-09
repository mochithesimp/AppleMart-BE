using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Model
{
    public class ProductSpecification
    {
        [Key]
        public int ProductSpecificationID { get; set; }

        [ForeignKey("ProductItem")]
        public int ProductItemID { get; set; }

        [MaxLength(255)]
        public string SpecificationName { get; set; }

        [MaxLength(1000)]
        public string SpecificationValue { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation property for ProductItem
        public ProductItem ProductItem { get; set; }
    }
}
