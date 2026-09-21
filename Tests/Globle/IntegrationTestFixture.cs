using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Globle
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public Task InitializeAsync()
        {
            var services = new ServiceCollection();

            var startup = new Startup();
            startup.ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            // Fix: Use IAsyncDisposable since DbContextScope implements asynchronous disposal
            if (ServiceProvider is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
