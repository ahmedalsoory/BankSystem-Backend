using Microsoft.AspNetCore.Mvc;
using Shared.Cache;

namespace API.Filter.Extensions
{
    public static class FilterExtensions
    {
        public static void AddBankingFilters(this MvcOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            // 1. Performance Measurement (Outermost wrapper)
            options.Filters.AddService<PerformanceFilter>(order: 1);

            // 2. Fail-Fast Validation (Short-circuits bad payloads BEFORE touching the DB pool)
            options.Filters.AddService<AutomaticFluentValidationFilter>(order: 2);

            // 3. DB Connection & Transaction Scope (Only opens for validated payloads)
            options.Filters.AddService<GlobalConnectionFilter>(order: 3);

         
        }

        public static void AddBankingFiltersServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddScoped<PerformanceFilter>();
            services.AddScoped<AutomaticFluentValidationFilter>();
            services.AddScoped<GlobalConnectionFilter>();
            services.AddScoped<ImageCommitActionFilter>();
        }
    }
}
