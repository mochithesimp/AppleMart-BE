using AutoMapper;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AttributeModel;
using iPhoneBE.Data.Models.CategoryModel;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.Models.ProductModel;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.AttributeDTO;
using iPhoneBE.Data.ViewModels.CategoryDTO;
using iPhoneBE.Data.ViewModels.ProductDTO;
using iPhoneBE.Data.ViewModels.ProductItemDTO;
using iPhoneBE.Data.ViewModels.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Category, CreateCategoryModel>().ReverseMap();
            CreateMap<Category, UpdateCategoryModel>().ReverseMap();

            CreateMap<Product, ProductViewModel>().ReverseMap();
            CreateMap<Product, CreateProductModel>().ReverseMap();
            CreateMap<Product, UpdateProductModel>().ReverseMap();

            CreateMap<ProductItem, ProductItemViewModel>().ReverseMap();
            CreateMap<ProductItem, CreateProductItemModel>().ReverseMap();
            CreateMap<ProductItem, UpdateProductItemModel>().ReverseMap();

            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ReverseMap(); // Role cần xử lý riêng
            CreateMap<UserModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Entities.Attribute, AttributeViewModel>().ReverseMap();
            CreateMap<Entities.Attribute, CreateAttributeModel>().ReverseMap();
            CreateMap<Entities.Attribute, UpdateAttributeModel>().ReverseMap();
        }
    }
}
