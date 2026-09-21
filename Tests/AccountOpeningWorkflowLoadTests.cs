using NBomber.Contracts;
using NBomber.CSharp;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Tests.Helper;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using DTOs.AccountOpeningDetail;
using ServiceContract.AccountOpeningDetails;
using Shared;
using Tests.Globle;


namespace Tests
{
    public class AccountOpeningWorkflowLoadTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITestOutputHelper _output;

        public AccountOpeningWorkflowLoadTests(IntegrationTestFixture fixture, ITestOutputHelper output)
        {
            _serviceProvider = fixture.ServiceProvider;
            _output = output;
        }

        [Fact]
        public void RunAccountOpeningWorkflowLoadTest()
        {
            int failureCount = 0;
            var runSessionId = Guid.NewGuid().ToString("N").Substring(0, 6);

            var scenario = Scenario.Create("account_opening_workflow_load", async context =>
            {
                string currentStep = "Initialization";
                var invocationId = context.InvocationNumber;

                // WRAPPED AROUND THE ENTIRE BODY: Guarantees connection release and scope 
                // disposal on EVERY exit path (Success, Validation Fail, or Exception)
                await using var scope = _serviceProvider.CreateAsyncScope();

                try
                {
                    currentStep = "Service Resolution & Scope Creation";
                    var clientWriteService = scope.ServiceProvider.GetRequiredService<IClientWriteService>();
                    var appWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
                    var detailsWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningDetailsWriteService>();
                    var orchestrator = scope.ServiceProvider.GetRequiredService<IAccountOpeningOrchestrator>();
                    var connectionProvider = scope.ServiceProvider.GetRequiredService<IDbConnectionProvider>();
                    var dbContextScope = scope.ServiceProvider.GetService<IDbContextScope>();

                    // 1. Client Creation (Using unique invocation ID to prevent duplicate key collisions)
                    currentStep = "Client Creation";
                    var clientResult = await new PersonBuilder()
                        .WithName("LoadTestUser", $"{runSessionId}_{invocationId}")
                        .WithEmail($"user_{runSessionId}_{invocationId}@bank.com")
                        .AsClient()
                        .BuildAsync(clientWriteService);

                    if (!clientResult.Success)
                    {
                        var errorMsg = $"Client creation failed: {string.Join(", ", clientResult.Errors)}";
                        LogFailure(ref failureCount, errorMsg);
                        return Response.Fail<object>(message: errorMsg);
                    }

                    // 2. Application Creation
                    currentStep = "Application Creation";
                    var appResult = await new AccountOpeningApplicationBuilder()
                        .ForClient(clientResult.Data)
                        .WithOnboardingType(2)
                        .WithDeposit(4000.00f)
                        .BuildAsync(appWriteService);

                    if (!appResult.Success)
                    {
                        var errorMsg = $"Application creation failed: {string.Join(", ", appResult.Errors)}";
                        LogFailure(ref failureCount, errorMsg);
                        return Response.Fail<object>(message: errorMsg);
                    }

                    int applicationId = appResult.Data;

                    // 3. Fetch Workflow Steps
                    currentStep = "Fetch Workflow Steps";
                    var workflowSteps = await WorkflowTestHelper.GetStepsByApplicationIdAsync(applicationId, connectionProvider);
                    if (workflowSteps == null || !workflowSteps.Any())
                    {
                        var errorMsg = "Workflow steps could not be fetched for Application ID: " + applicationId;
                        LogFailure(ref failureCount, errorMsg);
                        return Response.Fail<object>(message: errorMsg);
                    }

                    int totalSteps = workflowSteps.Count();
                    int currentStepIndex = 1;

                    // 4. Loop Through Workflow Steps
                    foreach (var step in workflowSteps)
                    {
                        currentStep = $"Submit Detail ({step.StepName})";
                        var detailRequest = new AccountOpeningDetailRequest
                        {
                            ApplicationID = applicationId,
                            RequirementKey = step.StepName,
                            RequirementValue = $"VerifiedValue_{step.StepName}"
                        };

                        var detailResult = await detailsWriteService.AddDetailAsync(detailRequest);
                        if (!detailResult.Success)
                        {
                            var errorMsg = $"Adding detail for step '{step.StepName}' failed: {string.Join(", ", detailResult.Errors)}";
                            LogFailure(ref failureCount, errorMsg);
                            return Response.Fail<object>(message: errorMsg);
                        }

                        int detailId = detailResult.Data;

                        currentStep = $"Step Verification ({step.StepName})";
                        var verificationRequest = new VerifiedStatusRequestBuilder()
                            .ForDetail(detailId)
                            .ForApplication(applicationId)
                            .ForStep(step.StepName)
                            .ByEmployee(1)
                            .Build();

                        var orchestratorResult = await orchestrator.ProcessStatusVerificationAsync(verificationRequest);
                        if (!orchestratorResult.Success)
                        {
                            var errorMsg = $"Verification step '{step.StepName}' failed: {string.Join(", ", orchestratorResult.Errors)}";
                            LogFailure(ref failureCount, errorMsg);
                            return Response.Fail<object>(message: errorMsg);
                        }

                        if (currentStepIndex < totalSteps)
                        {
                            if (orchestratorResult.Data)
                            {
                                var errorMsg = $"Intermediate step '{step.StepName}' prematurely triggered completion.";
                                LogFailure(ref failureCount, errorMsg);
                                return Response.Fail<object>(message: errorMsg);
                            }
                        }
                        else
                        {
                            if (!orchestratorResult.Data)
                            {
                                var errorMsg = $"Final step '{step.StepName}' failed to trigger auto-creation.";
                                LogFailure(ref failureCount, errorMsg);
                                return Response.Fail<object>(message: errorMsg);
                            }
                        }

                        currentStepIndex++;
                    }

                    dbContextScope?.Commit();
                    return Response.Ok();
                }
                catch (Exception ex)
                {
                    var errorMsg = $"Exception at [{currentStep}]: {ex.Message} -> {ex.InnerException?.Message}";
                    LogFailure(ref failureCount, errorMsg);
                    return Response.Fail<object>(message: errorMsg);
                }
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
            );

            // Run NBomber
            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.ScenarioStats.First(s => s.ScenarioName == "account_opening_workflow_load");

            _output.WriteLine($"=== NBomber Performance Results ===");
            _output.WriteLine($"Total Successful Requests: {scnStats.Ok.Request.Count}");
            _output.WriteLine($"Requests Per Second (RPS): {scnStats.Ok.Request.RPS}");
            _output.WriteLine($"Mean Latency: {scnStats.Ok.Latency.MeanMs} ms");
            _output.WriteLine($"P95 Latency: {scnStats.Ok.Latency.Percent95} ms");
            _output.WriteLine($"Failed Requests: {scnStats.Fail.Request.Count}");
        }

        private void LogFailure(ref int failureCount, string errorMsg)
        {
            int currentFailures = Interlocked.Increment(ref failureCount);
            if (currentFailures <= 5)
            {
                _output.WriteLine($"[NBOMBER ERROR] {errorMsg}");
            }
        }
    }
}