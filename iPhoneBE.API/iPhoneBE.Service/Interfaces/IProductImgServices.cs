using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductImgModel;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.ViewModels.ProductImgVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Interfaces
{
    public interface IProductImgServices
    {
        Task<IEnumerable<ProductImg>> GetAllAsync();

        Task<List<ProductImg>> AddMultipleAsync(CreateProductImgModel model);
    }
}
