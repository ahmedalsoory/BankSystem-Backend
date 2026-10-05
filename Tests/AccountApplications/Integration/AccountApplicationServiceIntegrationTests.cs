using Microsoft.Extensions.DependencyInjection;
using ServiceContract.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;

namespace Tests.AccountApplications.Integration
{
    public partial class AccountApplicationServiceIntegrationTests : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        protected IAccountApplicationReadService ReadService => _scopedServiceProvider.GetRequiredService<IAccountApplicationReadService>();
        protected IAccountApplicationWriteService WriteService => _scopedServiceProvider.GetRequiredService<IAccountApplicationWriteService>();
        protected IAccountApplicationReadTransactionService ReadTransactionService => _scopedServiceProvider.GetRequiredService<IAccountApplicationReadTransactionService>();

        public AccountApplicationServiceIntegrationTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }
    }
}
