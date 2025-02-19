using iPhoneBE.Data.Interfaces;
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

        public static IServiceCollection AddRepository(this IServiceCollection service)
        {
            service.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            service.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            service.AddScoped<IAccountRepository, AccountRepository>();

            return service;
        }
    }
}
