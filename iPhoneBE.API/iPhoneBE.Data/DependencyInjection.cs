using iPhoneBE.Data.Helper.EmailHelper;
using iPhoneBE.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddRepository(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            service.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            service.Configure<EmailConfiguration>(configuration.GetSection("MailSettings"));
            //service.Configure<EmailConfiguration>(options => configuration.GetSection("MailSettings").Bind(options));
            service.AddScoped<IEmailHelper, EmailHelper>();

            return service;
        }
    }
}
