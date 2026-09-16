using Microsoft.AspNetCore.Mvc;
using Shared.Cache;

namespace API.Filter.Extensions
{
    public static class FilterExtensions
    {
        public static void AddBankingFilters(this MvcOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
          //  options.Filters.AddService<GlobalStrictQueryFilter>();
            options.Filters.AddService<AutomaticFluentValidationFilter>();
            options.Filters.AddService<PerformanceFilter>();
        //    options.Filters.AddService<CacheVersionFilter>();
            options.Filters.AddService<TransactionExceptionFilter>();
            options.Filters.AddService<ImageCommitActionFilter>();
            options.Filters.AddService<GlobalConnectionFilter>(order: 999);
        }
        public static void AddBankingFiltersServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
         //   services.AddScoped<GlobalStrictQueryFilter>();
            services.AddScoped<AutomaticFluentValidationFilter>();
            services.AddScoped<PerformanceFilter>();
          //  services.AddScoped<CacheVersionFilter>();
            services.AddScoped<TransactionExceptionFilter>();
            services.AddScoped<ImageCommitActionFilter>();
            services.AddScoped<GlobalConnectionFilter>();
        }
    }
}
