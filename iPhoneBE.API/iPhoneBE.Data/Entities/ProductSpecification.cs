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
        [Required(ErrorMessage = "Product Item ID is required.")]
        public int ProductItemID { get; set; }

        [MaxLength(255, ErrorMessage = "Specification name cannot exceed 255 characters.")]
        public string? SpecificationName { get; set; }

        [MaxLength(1000, ErrorMessage = "Specification value cannot exceed 1000 characters.")]
        public string? SpecificationValue { get; set; }
        public bool IsDeleted { get; set; }
        public ProductItem ProductItem { get; set; }
    }
}
