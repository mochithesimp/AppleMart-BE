using System.ComponentModel.DataAnnotations;

namespace iPhoneBE.Data.Models.PaypalModel
{
    public class CreatePaypalTransactionModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string PaypalPaymentId { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public float Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}