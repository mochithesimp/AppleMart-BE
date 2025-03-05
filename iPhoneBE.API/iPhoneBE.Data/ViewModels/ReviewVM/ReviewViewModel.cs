namespace iPhoneBE.Data.ViewModels.ReviewVM
{
    public class ReviewViewModel
    {
        public int ReviewID { get; set; }
        public string UserID { get; set; }
        public int OrderDetailID { get; set; }
        public int ProductItemID { get; set; }
        public string? ShipperID { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string UserName { get; set; }
        public string? ShipperName { get; set; }
    }
}