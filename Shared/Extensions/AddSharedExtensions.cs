using Microsoft.Extensions.DependencyInjection;
using Shared.Cache;
using Shared.Image;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    public static class AddSharedExtensions
    {
        /// <summary>
        /// Registers shared infrastructure services (DbContextScope, Audit Tracker, etc.) into the DI container.
        /// </summary>
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {

            services.AddScoped<DbContextScope>();
            services.AddScoped<IDbContextScope>(sp => sp.GetRequiredService<DbContextScope>());

            services.AddScoped<IDbConnectionProvider>(sp => sp.GetRequiredService<DbContextScope>());
       //     services.AddSingleton<ICacheVersionService, CacheVersionService>();

            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IImageTracker, ImageTracker>();
            services.AddScoped<IImageHandler, ImageHandler>();

            services.AddScoped<Context>();

            services.AddScoped<IAuditTracker, AuditTracker>();

            return services;
        }
    }
}
