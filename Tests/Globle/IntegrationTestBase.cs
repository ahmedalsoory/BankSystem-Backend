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
        protected IDbContextScope _dbContextScope;
        private bool _isTestSuccessful = false;

        protected IntegrationTestBase(IntegrationTestFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        public async Task InitializeAsync()
        {
            _dbContextScope = _serviceProvider.GetRequiredService<IDbContextScope>();
            await _dbContextScope.BeginTransactionAsync();
            _isTestSuccessful = false; // Reset for each test
        }

        // Call this at the very end of your test if all assertions pass
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
                        // If the test reached the end successfully, commit the transaction
                        _dbContextScope.Commit();
                    }
                    else
                    {
                        // If an assertion failed or an exception occurred, roll back
                        _dbContextScope.Rollback();
                    }
                }
                finally
                {
                    await _dbContextScope.DisposeAsync();
                }
            }
        }
    }
}
