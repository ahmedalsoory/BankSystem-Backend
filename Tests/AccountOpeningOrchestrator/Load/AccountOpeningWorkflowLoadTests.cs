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


namespace Tests.AccountOpeningOrchestrator.Load
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
                var invocationId = context.InvocationNumber;
                string firstName = "LoadTestUser";
                string lastName = $"{runSessionId}_{invocationId}";
                string email = $"user_{runSessionId}_{invocationId}@bank.com";

                // Call the shared helper
                var result = await AccountWorkflowExecutionHelper.ExecuteWorkflowAsync(_serviceProvider, firstName, lastName, email);

                if (!result.Success)
                {
                    LogFailure(ref failureCount, result.Message);
                    return Response.Fail<object>(message: result.Message);
                }

                return Response.Ok();
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