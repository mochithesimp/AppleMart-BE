using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Model
{
    public class ShippingMethod
    {
        [Key]
        public int ShippingMethodID { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }
        public float ShippingPrice { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
