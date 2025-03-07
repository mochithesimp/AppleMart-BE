using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.ViewModels.ProductItemVM
{
    public class ProductItemSalesViewModel
    {
        public int ProductItemId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int TotalSold { get; set; }
    }
}
