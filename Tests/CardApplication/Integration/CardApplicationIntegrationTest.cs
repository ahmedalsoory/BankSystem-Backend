using Microsoft.Extensions.DependencyInjection;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.CardApplication;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;

namespace Tests.CardApplication.Integration
{
  public partial class CardApplicationIntegrationTest : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        // Use expression-bodied properties so they tie into the per-test scope automatically
        protected ICardApplicationWriteService _cardApplicationWriteService => _scopedServiceProvider.GetRequiredService<ICardApplicationWriteService>();
        protected IClientWriteService _clientWriteService => _scopedServiceProvider.GetRequiredService<IClientWriteService>();
        protected IAccountOpeningApplicationsWriteService _accountOpeningApplicationsWriteService => _scopedServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
        protected IAccountOpeningDetailsWriteService _accountOpeningDetailsWriteService => _scopedServiceProvider.GetRequiredService<IAccountOpeningDetailsWriteService>();
        protected IAccountOpeningOrchestrator _accountOpeningOrchestrator => _scopedServiceProvider.GetRequiredService<IAccountOpeningOrchestrator>();
        
        // If you need the connection/context provider, expose it like this:
        protected IDbContextScope _connectionProvider => _dbContextScope;

        public CardApplicationIntegrationTest(IntegrationTestFixture fixture) : base(fixture)
        {
            // Constructor stays clean. No manual scope creation here.
        }
    }
}
