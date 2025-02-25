namespace iPhoneBE.Data.ViewModels.ProductImgDTO
{
    public class ProductImgViewModel
    {
        public int ProductImgID { get; set; }
        public int ProductItemID { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
    }
}