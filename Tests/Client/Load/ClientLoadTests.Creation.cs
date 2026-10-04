using Microsoft.AspNetCore.Mvc.Testing;
using NBomber.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Client.Load
{
    public partial class ClientLoadTests
    {
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
                    { new StringContent($"151515{(uniqueId % 100000 + 10220000)}2253242{runSessionId}"), "Phone" },
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
                Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.ScenarioStats.First(s => s.ScenarioName == "client_creation_scenario");

            _output.WriteLine($"=== NBomber Creation Performance Results ===");
            _output.WriteLine($"Total Successful Requests: {scnStats.Ok.Request.Count}");
            _output.WriteLine($"Requests Per Second (RPS): {scnStats.Ok.Request.RPS}");
            _output.WriteLine($"Mean Latency: {scnStats.Ok.Latency.MeanMs} ms");
            _output.WriteLine($"P95 Latency: {scnStats.Ok.Latency.Percent95} ms");
            _output.WriteLine($"Failed Requests: {scnStats.Fail.Request.Count}");
        }


        [Fact]
        public void RunClientCreationLoadTest_withImage()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var runSessionId = Guid.NewGuid().ToString("N").Substring(0, 6);
            int failureCount = 0;

            // Minimal valid 1x1 PNG byte array for rapid load testing in memory
            byte[] dummyPngBytes = new byte[]
            {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4, 0x89, 0x00, 0x00, 0x00,
                0x0A, 0x49, 0x44, 0x41, 0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
                0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00, 0x00, 0x00, 0x00, 0x49,
                0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

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
                    { new StringContent($"00{(uniqueId % 10111333 + 10221110022)}00{runSessionId}"), "Phone" },
                    { new StringContent("1990-01-01"), "BirthDate" },
                    { new StringContent("M"), "Gendor" }
                };

                // Add real image binary content mapped to parameter name "profileImage"
                var imageContent = new ByteArrayContent(dummyPngBytes);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                content.Add(imageContent, "profileImage", "profile.png");

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
                // Phase 1: Warm up ThreadPool (0 -> 50 RPS over 5s)
                Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)),

                // Phase 2: Sustained Load at 100 RPS for 15s
                Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)),

                // Phase 3: Stress Peak at 200 RPS for 15s
                Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.ScenarioStats.First(s => s.ScenarioName == "client_creation_scenario");

            _output.WriteLine($"=== NBomber High Concurrency Results ===");
            _output.WriteLine($"Total Successful Requests: {scnStats.Ok.Request.Count}");
            _output.WriteLine($"Requests Per Second (RPS): {scnStats.Ok.Request.RPS}");
            _output.WriteLine($"Mean Latency: {scnStats.Ok.Latency.MeanMs} ms");
            _output.WriteLine($"P95 Latency: {scnStats.Ok.Latency.Percent95} ms");
            _output.WriteLine($"P99 Latency: {scnStats.Ok.Latency.Percent99} ms");
            _output.WriteLine($"Failed Requests: {scnStats.Fail.Request.Count}");
        }
    }
}

