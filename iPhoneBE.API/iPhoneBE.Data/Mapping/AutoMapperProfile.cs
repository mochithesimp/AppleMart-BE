using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Migrations;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AttributeModel;
using iPhoneBE.Data.Models.AuthenticationModel;
using iPhoneBE.Data.Models.BlogImageModel;
using iPhoneBE.Data.Models.BlogModel;
using iPhoneBE.Data.Models.CategoryModel;
using iPhoneBE.Data.Models.ProductImgModel;
using iPhoneBE.Data.Models.ProductItemAttributeModel;
using iPhoneBE.Data.Models.ProductItemModel;
using iPhoneBE.Data.Models.ProductModel;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.AttributeVM;
using iPhoneBE.Data.ViewModels.BlogVM;
using iPhoneBE.Data.ViewModels.CategoryVM;
using iPhoneBE.Data.ViewModels.ChatDTO;
using iPhoneBE.Data.ViewModels.ChatVM;
using iPhoneBE.Data.ViewModels.OrderVM;
using iPhoneBE.Data.ViewModels.ProductImgVM;
using iPhoneBE.Data.ViewModels.ProductItemAttributeVM;
using iPhoneBE.Data.ViewModels.ProductItemVM;
using iPhoneBE.Data.ViewModels.ProductVM;
using iPhoneBE.Data.ViewModels.UserVM;

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
            CreateMap<RegisterModel, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Map Email -> UserName
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
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

            CreateMap<ChatMessage, ChatMessageViewModel>()
                .ForMember(dest => dest.ChatID, opt => opt.MapFrom(src => src.ChatMessageID))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.UserName : "Unknown User"));

            CreateMap<ChatRoom, ChatRoomViewModel>()
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.ChatMessages))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.ChatParticipants))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
                    src.ChatMessages.OrderByDescending(m => m.CreatedDate).FirstOrDefault()));

            CreateMap<ChatParticipant, ChatParticipantViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.UserName : "Unknown User"))
                .ForMember(dest => dest.IsOnline, opt => opt.Ignore());

            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderDetail, OrderDetailViewModel>();
        }
    }
}
