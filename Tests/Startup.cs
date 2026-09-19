using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Shared.Extensions;
using Service.Extension_Method;
using Repositories.Extensions;


namespace Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true)
                 .AddEnvironmentVariables()
                 .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddHttpContextAccessor();

            // 2. Register shared infrastructure (DbContextScope, IDbConnectionProvider, Context, AuditTracker, etc.)
            services.AddSharedServices();

            // 3. Register all Repositories & Services using your existing extensions
            services.AddBankRepositories();
            services.AddBankService();
        }
    }
}
