using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Globle
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected readonly IServiceProvider _serviceProvider;
        protected AsyncServiceScope _testScope; // <-- Hold the scope here
        protected IDbContextScope _dbContextScope;
        protected IServiceProvider _scopedServiceProvider; // <-- Use this for resolving services
        private bool _isTestSuccessful = false;

        protected IntegrationTestBase(IntegrationTestFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        public async Task InitializeAsync()
        {
            // 1. Create ONE scope per test execution
            _testScope = _serviceProvider.CreateAsyncScope();
            _scopedServiceProvider = _testScope.ServiceProvider;

            // 2. Resolve the transaction scope from THAT SAME scope
            _dbContextScope = _scopedServiceProvider.GetRequiredService<IDbContextScope>();
            await _dbContextScope.BeginTransactionAsync();
            _isTestSuccessful = false;
        }

        protected void MarkTestAsSuccessful()
        {
            _isTestSuccessful = true;
        }

        public async Task DisposeAsync()
        {
            if (_dbContextScope != null)
            {
                try
                {
                    if (_isTestSuccessful)
                    {
                        _dbContextScope.Commit();
                    }
                    else
                    {
                        _dbContextScope.Rollback();
                    }
                }
                finally
                {
                    await _dbContextScope.DisposeAsync();
                    // Dispose the test scope as well to clean up services
                    await _testScope.DisposeAsync();
                }
            }
        }
    }
}
