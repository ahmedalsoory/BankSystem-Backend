using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.Client;
using Shared.Interfaces;
using DTOs.AccountOpeningDetail;
using Tests.Helper;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;

namespace Tests.Helper
{
    public static class AccountWorkflowExecutionHelper
    {
        /// <summary>
        /// Executes the full account opening workflow inside a given service provider scope.
        /// Throws an exception or returns failure details if any step fails.
        /// </summary>
        public static async Task<Result<bool>> ExecuteWorkflowAsync(
            IServiceProvider serviceProvider,
            string firstName,
            string lastName,
            string email,
            float initailDeposit = 4000)
        {

            await using var scope = serviceProvider.CreateAsyncScope();

            var clientWriteService = scope.ServiceProvider.GetRequiredService<IClientWriteService>();
            var appWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
            var detailsWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningDetailsWriteService>();
            var orchestrator = scope.ServiceProvider.GetRequiredService<IAccountOpeningOrchestrator>();
            var connectionProvider = scope.ServiceProvider.GetRequiredService<IDbConnectionProvider>();
            var dbContextScope = scope.ServiceProvider.GetService<IDbContextScope>();

            // 1. Client Creation
            var clientResult = await new PersonBuilder()
                .WithName(firstName, lastName)
                .WithEmail(email)
                .AsClient()
                .BuildAsync(clientWriteService);

            if (!clientResult.Success)
                return Result<bool>.Fail($"Client creation failed: {string.Join(", ", clientResult.Errors)}");

            // 2. Application Creation
            var appResult = await new AccountOpeningApplicationBuilder()
                .ForClient(clientResult.Data)
                .WithOnboardingType(2)
                
                .WithDeposit(initailDeposit)
                .BuildAsync(appWriteService);

            if (!appResult.Success)
                return Result<bool>.Fail($"Application creation failed: {string.Join(", ", appResult.Errors)}");

            int applicationId = appResult.Data;

            // 3. Fetch Workflow Steps
            var workflowSteps = await WorkflowTestHelper.GetStepsByApplicationIdAsync(applicationId, connectionProvider);
            if (workflowSteps == null || !workflowSteps.Any())
                return Result<bool>.Fail($"Workflow steps could not be fetched for Application ID: {applicationId}");

            int totalSteps = workflowSteps.Count();
            int currentStepIndex = 1;

            // 4. Loop Through Workflow Steps
            foreach (var step in workflowSteps)
            {
                var detailRequest = new AccountOpeningDetailRequest
                {
                    ApplicationID = applicationId,
                    RequirementKey = step.StepName,
                    RequirementValue = $"VerifiedValue_{step.StepName}"
                };

                var detailResult = await detailsWriteService.AddDetailAsync(detailRequest);
                if (!detailResult.Success)
                    return Result<bool>.Fail($"Adding detail for step '{step.StepName}' failed: {string.Join(", ", detailResult.Errors)}");

                int detailId = detailResult.Data;

                var verificationRequest = new VerifiedStatusRequestBuilder()
                    .ForDetail(detailId)
                    .ForApplication(applicationId)
                    .ForStep(step.StepName)
                    .ByEmployee(1)
                    .Build();

                var orchestratorResult = await orchestrator.ProcessStatusVerificationAsync(verificationRequest);
                if (!orchestratorResult.Success)
                    return Result<bool>.Fail($"Verification step '{step.StepName}' failed: {string.Join(", ", orchestratorResult.Errors)}");

                if (currentStepIndex < totalSteps && orchestratorResult.Data)
                    return Result<bool>.Fail($"Intermediate step '{step.StepName}' prematurely triggered completion.");

                if (currentStepIndex == totalSteps && !orchestratorResult.Data)
                    return Result<bool>.Fail($"Final step '{step.StepName}' failed to trigger auto-creation.");

                currentStepIndex++;
            }

           // dbContextScope?.Commit();
            return Result<bool>.Ok(true);
        }

        public class WorkflowExecutionResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public int ClientId { get; set; }
            public int ApplicationId { get; set; }
            public int AccountId { get; set; } // Populated if the final step creates an account

            public static WorkflowExecutionResult Fail(string error) => new() { Success = false, ErrorMessage = error };
        }
        // Notice: NO scope creation here. It uses whatever services are handed to it!
        public static async Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
            IClientWriteService clientWriteService,
            IAccountOpeningApplicationsWriteService appWriteService,
            IAccountOpeningDetailsWriteService detailsWriteService,
            IAccountOpeningOrchestrator orchestrator,
            IDbConnectionProvider connectionProvider,
            string firstName,
            string lastName,
            string email,
            float initailDeposit = 4000)
        {
            // 1. Client Creation
            var clientResult = await new PersonBuilder()
                .WithName(firstName, lastName)
                .WithEmail(email)
                .AsClient()
                .BuildAsync(clientWriteService);

            if (!clientResult.Success)
                return WorkflowExecutionResult.Fail($"Client creation failed: {string.Join(", ", clientResult.Errors)}");

            int clientId = clientResult.Data;

            // 2. Application Creation
            var appResult = await new AccountOpeningApplicationBuilder()
                .ForClient(clientId)
                .WithOnboardingType(2)
                .WithDeposit(initailDeposit)
                .BuildAsync(appWriteService);

            if (!appResult.Success)
                return WorkflowExecutionResult.Fail($"Application creation failed: {string.Join(", ", appResult.Errors)}");

            int applicationId = appResult.Data;

            // 3. Fetch Workflow Steps & Loop (Runs on the shared connection)
            var workflowSteps = await WorkflowTestHelper.GetStepsByApplicationIdAsync(applicationId, connectionProvider);
            if (workflowSteps == null || !workflowSteps.Any())
                return WorkflowExecutionResult.Fail("Workflow steps could not be fetched.");

            int currentStepIndex = 1;
            int totalSteps = workflowSteps.Count();
            int capturedAccountId = 0;

            foreach (var step in workflowSteps)
            {
                var detailRequest = new AccountOpeningDetailRequest
                {
                    ApplicationID = applicationId,
                    RequirementKey = step.StepName,
                    RequirementValue = $"VerifiedValue_{step.StepName}"
                };

                var detailResult = await detailsWriteService.AddDetailAsync(detailRequest);
                if (!detailResult.Success)
                    return WorkflowExecutionResult.Fail("Adding detail failed.");

                var verificationRequest = new VerifiedStatusRequestBuilder()
                    .ForDetail(detailResult.Data)
                    .ForApplication(applicationId)
                    .ForStep(step.StepName)
                    .ByEmployee(1)
                    .Build();

                var orchestratorResult = await orchestrator.ProcessStatusVerificationAsync(verificationRequest);
                if (!orchestratorResult.Success)
                    return WorkflowExecutionResult.Fail("Verification failed.");

                currentStepIndex++;
            }

            return new WorkflowExecutionResult
            {
                Success = true,
                ClientId = clientId,
                ApplicationId = applicationId,
                AccountId = capturedAccountId
            };
        }
    }
}