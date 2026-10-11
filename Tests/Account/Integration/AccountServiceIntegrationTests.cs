using Microsoft.Extensions.DependencyInjection;
using ServiceContract.Account;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;

namespace Tests.Account.Integration
{
    public partial class AccountServiceIntegrationTests : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        // Resolve the service directly from the transaction-safe DI scope
        protected IAccountReadService AccountReadService => _scopedServiceProvider.GetRequiredService<IAccountReadService>();
        protected IAccountWriteService AccountWriteService => _scopedServiceProvider.GetRequiredService<IAccountWriteService>();
        protected IDbContextScope _connectionProvider => _dbContextScope;
        public AccountServiceIntegrationTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }
    }
}
