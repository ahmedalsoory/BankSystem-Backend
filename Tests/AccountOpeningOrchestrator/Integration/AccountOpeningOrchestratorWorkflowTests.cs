using Microsoft.Extensions.DependencyInjection;
using ServiceContract.Account;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;

namespace Tests.AccountOpeningOrchestrator.Integration
{
    public partial class AccountOpeningOrchestratorWorkflowTests : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        // ✅ Use expression-bodied properties to pull services from the base class's shared scope
        protected IClientWriteService _clientWriteService => _scopedServiceProvider.GetRequiredService<IClientWriteService>();
        protected IAccountOpeningApplicationsWriteService _accountOpeningApplicationsWriteService => _scopedServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
        protected IAccountOpeningDetailsWriteService _accountOpeningDetailsWriteService => _scopedServiceProvider.GetRequiredService<IAccountOpeningDetailsWriteService>();
        protected IAccountOpeningOrchestrator _accountOpeningOrchestrator => _scopedServiceProvider.GetRequiredService<IAccountOpeningOrchestrator>();
        protected IAccountReadService _accountReadService => _scopedServiceProvider.GetRequiredService<IAccountReadService>();
        protected IDbConnectionProvider _connectionProvider => _scopedServiceProvider.GetRequiredService<IDbConnectionProvider>();

        public AccountOpeningOrchestratorWorkflowTests(IntegrationTestFixture fixture) : base(fixture)
        {
            // ❌ REMOVED: var scope = _serviceProvider.CreateScope(); and manual service assignments.
            // The base class now handles the scope and transaction initialization automatically!
        }
    }
}
