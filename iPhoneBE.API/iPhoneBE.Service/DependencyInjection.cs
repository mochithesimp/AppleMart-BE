using iPhoneBE.Data.Interfaces;
using iPhoneBE.Service.Interfaces;
using iPhoneBE.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddScoped<ICategoryServices, CategoryServices>();
            service.AddScoped<IAccountServices, AccountServices>();
            service.AddScoped<IUserServices, UserServices>();
            service.AddScoped<IBlogServices, BlogServices>();
            service.AddScoped<IAttributeServices, AttributeServices>();
            service.AddScoped<IProductItemAttributeServices, ProductItemAttributeServices>();
            service.AddScoped<IProductServices, ProductServices>();
            service.AddScoped<IProductItemServices, ProductItemServices>();
            service.AddScoped<IChatServices, ChatServices>();
            service.AddScoped<IOrderServices, OrderServices>();
            service.AddScoped<IAdminServices, AdminServices>();
            service.AddScoped<INotificationServices, NotificationServices>();
            service.AddScoped<IProductImgServices, ProductImgServices>();
            service.AddScoped<IPaypalTransactionServices, PaypalTransactionServices>();

            return service;
        }
    }
}