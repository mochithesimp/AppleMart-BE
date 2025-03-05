namespace iPhoneBE.Data.ViewModels.ProductItemAttributeVM
{
    public class ProductItemAttributeViewModel
    {
        public int ProductItemAttributeID { get; set; }
        public int ProductItemID { get; set; }
        public int AttributeID { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }
    }
}