using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.ViewModels.ProductVM;
using iPhoneBE.Data.ViewModels.ProductItemAttributeVM;
using iPhoneBE.Data.ViewModels.ProductImgVM;

namespace iPhoneBE.Data.ViewModels.ProductItemVM
{
    public class ProductItemViewModel
    {
        public int ProductItemID { get; set; }

        public int ProductID { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public int DisplayIndex { get; set; }

        public double Price { get; set; }

        public bool IsDeleted { get; set; }

        public ProductViewModel Product { get; set; }
        public ICollection<ProductImgViewModel> ProductImgs { get; set; }
        public ICollection<ProductItemAttributeViewModel> ProductItemAttributes { get; set; }
    }
}
