using NBomber.CSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Tests
{
    public class ClientLoadTests
    {
        private readonly ITestOutputHelper _output;

        public ClientLoadTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void RunClientCreationLoadTest()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var runSessionId = Guid.NewGuid().ToString("N").Substring(0, 6);
            int failureCount = 0;

            var scenario = Scenario.Create("client_creation_scenario", async context =>
            {
                var uniqueId = context.InvocationNumber;

                using var content = new MultipartFormDataContent
        {
            { new StringContent("1"), "RiskLevel" },
            { new StringContent("true"), "IsActive" },
            { new StringContent($"CN-{runSessionId}-{uniqueId}"), "ClientNumber" },
            { new StringContent($"UserFirst_{uniqueId}"), "FirstName" },
            { new StringContent($"UserLast_{uniqueId}"), "LastName" },
            { new StringContent($"NAT{runSessionId}{uniqueId}"), "NationalId" },
            { new StringContent($"user_{runSessionId}_{uniqueId}@bank.com"), "Email" },
            { new StringContent($"555{(uniqueId % 900000 + 100000)}"), "Phone" },
            { new StringContent("1990-01-01"), "BirthDate" },
            { new StringContent(""), "ImagePath" },
            { new StringContent("M"), "Gendor" }
        };

                var response = await httpClient.PostAsync("/api/Client/create", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    int currentFailures = Interlocked.Increment(ref failureCount);
                    if (currentFailures <= 5)
                    {
                        _output.WriteLine($"API Error [{response.StatusCode}]: {errorContent}");
                    }
                    return Response.Fail();
                }

                return Response.Ok();
            })
            .WithLoadSimulations(
                // Scale target rate to 300 requests per second for 30 seconds (Total = 9,000 requests)
                Simulation.Inject(rate: 300, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.ScenarioStats.First(s => s.ScenarioName == "client_creation_scenario");

            _output.WriteLine($"=== NBomber Performance Results ===");
            _output.WriteLine($"Total Successful Requests: {scnStats.Ok.Request.Count}");
            _output.WriteLine($"Requests Per Second (RPS): {scnStats.Ok.Request.RPS}");
            _output.WriteLine($"Mean Latency: {scnStats.Ok.Latency.MeanMs} ms");
            _output.WriteLine($"P95 Latency: {scnStats.Ok.Latency.Percent95} ms");
            _output.WriteLine($"Failed Requests: {scnStats.Fail.Request.Count}");
        }
    }
}