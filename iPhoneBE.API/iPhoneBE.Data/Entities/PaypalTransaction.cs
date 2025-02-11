using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class PaypalTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public string PaypalPaymentId { get; set; }
        public string Status { get; set; }
        public float Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual Order Order { get; set; }
    }
}
