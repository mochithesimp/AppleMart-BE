using AutoMapper;
using iPhoneBE.Data;
using iPhoneBE.Data.Data;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.ProductImgModel;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.ViewModels.ProductImgVM;
using iPhoneBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Services
{
    public class ProductImgServices : IProductImgServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductImgServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductImg>> GetAllAsync()
        {
            var result = await _unitOfWork.ProductImgRepository.GetAllAsync();

            result = result?
                .Where(r => r.IsDeleted == false)
                .ToList() ?? new List<ProductImg>();

            return result;
        }

        public async Task<List<ProductImg>> AddMultipleAsync(CreateProductImgModel model)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(model.ProductItemID);
                if (productItem == null)
                    throw new KeyNotFoundException($"ProductItem with ID {model.ProductItemID} not found.");

                foreach (var url in model.ImageUrl)
                {
                    if (string.IsNullOrWhiteSpace(url))
                        throw new ArgumentException("One or more ImageUrl values are invalid.");

                    var img = new ProductImg
                    {
                        ProductItemID = model.ProductItemID,
                        ImageUrl = url,
                        IsDeleted = false
                    };

                    await _unitOfWork.ProductImgRepository.AddAsync(img);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return (await _unitOfWork.ProductImgRepository
                    .GetAllAsync(p => p.ProductItemID == model.ProductItemID && !p.IsDeleted)).ToList();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

    }

}
