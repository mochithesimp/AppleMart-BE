using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.AttributeModel
{
    public class UpdateAttributeModel
    {
        [MaxLength(255, ErrorMessage = "Attribute name cannot exceed 255 characters.")]
        public string? AttributeName { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? DataType { get; set; }
    }
}