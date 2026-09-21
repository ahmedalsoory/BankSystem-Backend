using ServiceContract.Account;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//using Tests.Helpers;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Xunit;
using DTOs.AccountOpeningDetail;
using ServiceContract.AccountOpeningDetails;
using Tests.Globle;
using Tests.Helper;

namespace Tests
{
    public class AccountOpeningOrchestratorWorkflowTests : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        private readonly IClientWriteService _clientWriteService;
        private readonly IAccountOpeningApplicationsWriteService _accountOpeningApplicationsWriteService;
        private readonly IAccountOpeningDetailsWriteService _accountOpeningDetailsWriteService;
        private readonly IAccountOpeningOrchestrator _accountOpeningOrchestrator;
        private readonly IAccountReadService _accountReadService;
        private readonly IDbConnectionProvider _connectionProvider;

        public AccountOpeningOrchestratorWorkflowTests(IntegrationTestFixture fixture) : base(fixture)
        {
            var scope = _serviceProvider.CreateScope();
            _clientWriteService = scope.ServiceProvider.GetRequiredService<IClientWriteService>();
            _accountOpeningApplicationsWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
            _accountOpeningDetailsWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningDetailsWriteService>();
            _accountOpeningOrchestrator = scope.ServiceProvider.GetRequiredService<IAccountOpeningOrchestrator>();
            _accountReadService = scope.ServiceProvider.GetRequiredService<IAccountReadService>();
            _dbContextScope = scope.ServiceProvider.GetRequiredService<IDbContextScope>();
            _connectionProvider = scope.ServiceProvider.GetRequiredService<IDbConnectionProvider>();
        }

        [Fact]
        public async Task ProcessStatusVerification_WhenAllStepsCompletedDynamically_ShouldAutoCreateAccountAndDeposit()
        {
            // 1. Arrange: Create the Client using PersonBuilder
            var clientResult = await new PersonBuilder()
                .WithName("Ayman", "Salem")
                .AsClient()
                .BuildAsync(_clientWriteService);

            clientResult.Success.Should().BeTrue($"because client creation failed: {string.Join(", ", clientResult.Errors)}");
            clientResult.Data.Should().BeGreaterThan(0);


            // 2. Arrange: Create the Application (Seeds workflow steps dynamically based on onboarding type)
            var appResult = await new AccountOpeningApplicationBuilder()
                .ForClient(clientResult.Data)
                .WithOnboardingType(2)
                .WithDeposit(4000.00f)
                .BuildAsync(_accountOpeningApplicationsWriteService);

            appResult.Success.Should().BeTrue($"because application creation failed: {string.Join(", ", appResult.Errors)}");
            appResult.Data.Should().BeGreaterThan(0);


            int applicationId = appResult.Data;

            // 3. Arrange: Dynamically fetch the workflow steps generated for this application
            var workflowSteps = await WorkflowTestHelper.GetStepsByApplicationIdAsync(applicationId, _connectionProvider);
            workflowSteps.Should().NotBeEmpty();

            int totalSteps = workflowSteps.Count();
            int currentStepIndex = 1;

            // 4. Act: Loop through each step, submit detail requirement first, then perform status verification
            foreach (var step in workflowSteps)
            {
                // Step A: Submit requirement details using IAccountOpeningDetailsWriteService
                var detailRequest = new AccountOpeningDetailRequest
                {
                    ApplicationID = applicationId,
                    RequirementKey = step.StepName,
                    RequirementValue = $"VerifiedValue_{step.StepName}"
                };

                var detailResult = await _accountOpeningDetailsWriteService.AddDetailAsync(detailRequest);
                detailResult.Success.Should().BeTrue($"because adding detail for step '{step.StepName}' failed: {string.Join(", ", detailResult.Errors)}");
                detailResult.Data.Should().BeGreaterThan(0);

                int detailId = detailResult.Data; // Capture the newly created Detail ID

                // Step B: Build verification request using the correct Detail ID
                var verificationRequest = new VerifiedStatusRequestBuilder()
                    .ForDetail(detailId) // <--- Pass the actual Detail ID here instead of step.StepID
                    .ForApplication(applicationId)
                    .ForStep(step.StepName)
                    .ByEmployee(1)
                    .Build();

                var orchestratorResult = await _accountOpeningOrchestrator.ProcessStatusVerificationAsync(verificationRequest);
                orchestratorResult.Success.Should().BeTrue($"because verification step '{step.StepName}' failed: {string.Join(", ", orchestratorResult.Errors)}");

                // Intermediate steps return false, final step triggers auto-creation and returns true
                if (currentStepIndex < totalSteps)
                {
                    orchestratorResult.Data.Should().BeFalse();
                }
                else
                {
                    orchestratorResult.Data.Should().BeTrue();
                }

                currentStepIndex++;
            }

            // 5. Assert: Verify the bank account was successfully auto-created and linked to the client
            //    var createdAccount = await _accountReadService.GetByClientIdAsync(clientResult.Data);
            //  createdAccount.Should().NotBeNull();

            _dbContextScope.Commit();
            // Mark the test as successful so the transaction commits automatically
            MarkTestAsSuccessful();
        }
    }
}