using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Shared.Extensions;
using Service.Extension_Method;
using Repositories.Extensions;
using Shared;

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
                 .AddInMemoryCollection(new Dictionary<string, string?>
           {
               { "CardSecurity:SecretKey", "TestSecretKey_ForIntegrationTestingOnly_12345!" }
           })
          
                 .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddHttpContextAccessor();

            // 1. ADD THIS LINE: Required by services using ILogger<T>
            services.AddLogging();

            // 2. Register shared infrastructure (DbContextScope, IDbConnectionProvider, Context, AuditTracker, etc.)
            services.AddSharedServices();
            services.Configure<DbSettings>(configuration.GetSection("ConnectionStrings"));

            // 3. Register all Repositories & Services using your existing extensions
            services.AddBankRepositories();
            services.AddBankService();
        }
    }
}