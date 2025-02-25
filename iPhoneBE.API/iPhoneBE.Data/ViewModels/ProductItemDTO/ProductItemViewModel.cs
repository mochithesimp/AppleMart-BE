using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iPhoneBE.Data.Entities;

namespace iPhoneBE.Data.ViewModels.ProductItemDTO
{
    public class ProductItemViewModel
    {
        public int ProductItemID { get; set; }

        public int ProductID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public bool IsDeleted { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<ProductImg> ProductImgs { get; set; }
        public virtual ICollection<ProductItemAttribute> ProductItemAttributes { get; set; }
    }
}
