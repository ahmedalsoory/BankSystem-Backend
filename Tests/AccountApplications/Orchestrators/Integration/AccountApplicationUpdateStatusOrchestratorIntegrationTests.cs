using Microsoft.Extensions.DependencyInjection;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.Applications.Orchestrators;
using ServiceContract.CardApplication;
using ServiceContract.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;

namespace Tests.AccountApplications.Orchestrators.Integration
{
    public partial class AccountApplicationUpdateStatusOrchestratorIntegrationTests : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        protected IAccountApplicationUpdateStatusOrchestrators Orchestrator =>
            _scopedServiceProvider.GetRequiredService<IAccountApplicationUpdateStatusOrchestrators>();


        protected ICardApplicationWriteService _cardApplicationWriteService => _scopedServiceProvider.GetRequiredService<ICardApplicationWriteService>();
        protected IClientWriteService _clientWriteService => _scopedServiceProvider.GetRequiredService<IClientWriteService>();
        protected IAccountOpeningApplicationsWriteService _accountOpeningApplicationsWriteService => _scopedServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
        protected IAccountOpeningDetailsWriteService _accountOpeningDetailsWriteService => _scopedServiceProvider.GetRequiredService<IAccountOpeningDetailsWriteService>();
        protected IAccountOpeningOrchestrator _accountOpeningOrchestrator => _scopedServiceProvider.GetRequiredService<IAccountOpeningOrchestrator>();

        public AccountApplicationUpdateStatusOrchestratorIntegrationTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }
    }
}
