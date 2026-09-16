using API.Attributes;
using API.Filter;
using DTOs.Account;
using DTOs.Account.interfaces;
using DTOs.AccountApplications.interfaces;
using DTOs.Client;
using DTOs.interfaces;
using DTOs.Person.interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Repositories.Extensions;
using Repositories.Validations;
using Service.Extension_Method;
using Service.Validation;
using Service.Validation.Account;
using ServiceContract;
using Shared;
using Shared.Cache;
using Shared.Image;
using Shared.Interfaces;
namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
 
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
          
            services.AddHttpContextAccessor();
            services.AddBankService();
            services.AddBankRepositories();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();



            services.AddMemoryCache();
            services.AddCors((options) =>
             options.AddDefaultPolicy(policy =>
             {
                 policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("ETag"); 
             })

            );

            return services;
        }
    }
}
