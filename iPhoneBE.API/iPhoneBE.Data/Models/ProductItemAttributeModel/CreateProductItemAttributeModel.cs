using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.ProductItemAttributeModel
{
    public class CreateProductItemAttributeModel
    {
        [Required(ErrorMessage = "ProductItem ID is required.")]
        public int ProductItemID { get; set; }

        [Required(ErrorMessage = "Attribute ID is required.")]
        public int AttributeID { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        [MaxLength(255, ErrorMessage = "Value cannot exceed 255 characters.")]
        public string Value { get; set; }
    }
}