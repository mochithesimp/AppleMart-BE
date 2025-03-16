namespace iPhoneBE.Data.ViewModels.PaypalVM
{
    public class PaypalTransactionViewModel
    {
        public int TransactionID { get; set; }
        public int OrderId { get; set; }
        public string PaypalPaymentId { get; set; }
        public string Status { get; set; }
        public float Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}