﻿using iPhoneBE.Data.Interfaces;
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

            return service;
        }
    }
}
