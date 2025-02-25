using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AttributeModel;
using iPhoneBE.Data.Models.BlogImageModel;
using iPhoneBE.Data.Models.BlogModel;
using iPhoneBE.Data.Models.CategoryModel;
using iPhoneBE.Data.Models.ProductImgModel;
using iPhoneBE.Data.Models.ProductItemAttributeModel;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.Models.ProductModel;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.AttributeDTO;
using iPhoneBE.Data.ViewModels.BlogDTO;
using iPhoneBE.Data.ViewModels.CategoryDTO;
using iPhoneBE.Data.ViewModels.ProductDTO;
using iPhoneBE.Data.ViewModels.ProductImgDTO;
using iPhoneBE.Data.ViewModels.ProductItemAttributeDTO;
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

            CreateMap<Entities.Attribute, AttributeViewModel>().ReverseMap();
            CreateMap<Entities.Attribute, CreateAttributeModel>().ReverseMap();
            CreateMap<Entities.Attribute, UpdateAttributeModel>().ReverseMap();

            CreateMap<ProductItemAttribute, ProductItemAttributeViewModel>().ReverseMap();
            CreateMap<ProductItemAttribute, CreateProductItemAttributeModel>().ReverseMap();
            CreateMap<ProductItemAttribute, UpdateProductItemAttributeModel>().ReverseMap();

            CreateMap<Product, ProductViewModel>().ReverseMap();
            CreateMap<Product, CreateProductModel>().ReverseMap();
            CreateMap<Product, UpdateProductModel>().ReverseMap();

            CreateMap<ProductItem, ProductItemViewModel>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.ProductImgs, opt => opt.MapFrom(src => src.ProductImgs))
                .ForMember(dest => dest.ProductItemAttributes, opt => opt.MapFrom(src => src.ProductItemAttributes));

            CreateMap<CreateProductItemModel, ProductItem>()
                .ForMember(dest => dest.ProductImgs, opt => opt.MapFrom(src =>
                    src.ImageUrls.Select(url => new ProductImg
                    {
                        ImageUrl = url,
                        IsDeleted = false
                    }).ToList()))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

            CreateMap<UpdateProductItemModel, ProductItem>()
                .ForMember(dest => dest.ProductItemID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.ProductImgs, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.ProductItemAttributes, opt => opt.Ignore())
                .ForMember(dest => dest.Vouchers, opt => opt.Ignore());

            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ReverseMap(); // Role cần xử lý riêng
            CreateMap<UserModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ProductImg, ProductImgViewModel>().ReverseMap();
            CreateMap<CreateProductImgModel, ProductImg>()
                .ForMember(dest => dest.ProductImgID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));
            CreateMap<UpdateProductImgModel, ProductImg>()
                .ForMember(dest => dest.ProductImgID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ProductItemID, opt => opt.Ignore());

            CreateMap<Blog, BlogViewModel>();
            CreateMap<BlogImage, BlogImageViewModel>();
            CreateMap<CreateBlogModel, Blog>();
            CreateMap<CreateBlogImageModel, BlogImage>();
        }
    }
}
