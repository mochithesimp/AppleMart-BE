using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Helper
{
    public class OrderStatusHelper
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled"; // coming soon
        public const string Refunded = "Refunded"; // coming soon
    }
}
