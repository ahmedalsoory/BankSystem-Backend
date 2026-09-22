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
        protected readonly IClientWriteService _clientWriteService;
        protected readonly IAccountOpeningApplicationsWriteService _accountOpeningApplicationsWriteService;
        protected readonly IAccountOpeningDetailsWriteService _accountOpeningDetailsWriteService;
        protected readonly IAccountOpeningOrchestrator _accountOpeningOrchestrator;
        protected readonly IAccountReadService _accountReadService;
        protected readonly IDbConnectionProvider _connectionProvider;

        public AccountOpeningOrchestratorWorkflowTests(IntegrationTestFixture fixture) : base(fixture)
        {
            var scope = _serviceProvider.CreateScope();
            _clientWriteService = scope.ServiceProvider.GetRequiredService<IClientWriteService>();
            _accountOpeningApplicationsWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
            _accountOpeningDetailsWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningDetailsWriteService>();
            _accountOpeningOrchestrator = scope.ServiceProvider.GetRequiredService<IAccountOpeningOrchestrator>();
            _accountReadService = scope.ServiceProvider.GetRequiredService<IAccountReadService>(); // Keep your original resolution logic
            _dbContextScope = scope.ServiceProvider.GetRequiredService<IDbContextScope>();
            _connectionProvider = scope.ServiceProvider.GetRequiredService<IDbConnectionProvider>();
        }
    }
}
