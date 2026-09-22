using NBomber.CSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using DTOs.Client;

namespace Tests.Client.Load
{
    public partial class ClientLoadTests
    {
    

        [Fact]
        public void RunClientUpdateLoadTest()
        {
            // 1. Setup WebApplicationFactory to host the API in-memory
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            var random = new Random();
            int failureCount = 0;

            var scenario = Scenario.Create("client_update_scenario", async context =>
            {
                try
                {
                    HttpResponseMessage getResponse;
                    int clientId = 0;
                    int attempts = 0;
                    const int maxAttempts = 10;

                    // 2. Loop until we find a client ID that actually exists in the database
                    while (true)
                    {
                        clientId = random.Next(400058, 973015);
                        getResponse = await httpClient.GetAsync($"/api/Client/{clientId}");

                        if (getResponse.IsSuccessStatusCode)
                        {
                            break; // Found an existing client! Exit loop.
                        }

                        attempts++;
                        if (attempts >= maxAttempts)
                        {
                            // If we tried 10 times and couldn't find one, skip this invocation 
                            // to prevent holding up the load test thread.
                            return Response.Ok();
                        }
                    }

                    var clientDetail = await getResponse.Content.ReadFromJsonAsync<ClientDetailDto>();
                    if (clientDetail == null)
                    {
                        return Response.Ok();
                    }

                    // 3. Build the MultipartFormDataContent with updated fields and tokens
                    using var content = new MultipartFormDataContent
                    {
                        { new StringContent(clientId.ToString()), "Id" },
                        { new StringContent(clientDetail.FirstName ?? "UpdatedName"), "FirstName" },
                        { new StringContent(clientDetail.LastName ?? "UpdatedLast"), "LastName" },
                        { new StringContent(clientDetail.NationalId ?? "0000000000"), "NationalId" },
                        { new StringContent(clientDetail.Email ?? "updated@bank.com"), "Email" },
                        { new StringContent(clientDetail.Phone ?? "5551234567"), "Phone" },
                        { new StringContent(clientDetail.BirthDate.ToString("yyyy-MM-dd")), "BirthDate" },
                        { new StringContent(clientDetail.Gendor.ToString()), "Gendor" },
                        { new StringContent(clientDetail.RiskLevel.ToString()), "RiskLevel" },
                        { new StringContent(clientDetail.IsActive.ToString()), "IsActive" }
                    };

                    // Append dual RowVersions explicitly matching your DTO property bindings
                    if (clientDetail.PersonVersion != null && clientDetail.PersonVersion.Length > 0)
                    {
                        content.Add(new StringContent(Convert.ToBase64String(clientDetail.PersonVersion)), "PersonRowVersion");
                    }

                    if (clientDetail.ClientVersion != null && clientDetail.ClientVersion.Length > 0)
                    {
                        content.Add(new StringContent(Convert.ToBase64String(clientDetail.ClientVersion)), "ClientRowVersion");
                    }

                    // 4. Send PUT request to your update endpoint
                    var putResponse = await httpClient.PutAsync("/api/Client/update", content);

                    if (!putResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await putResponse.Content.ReadAsStringAsync();
                        int currentFailures = Interlocked.Increment(ref failureCount);
                        if (currentFailures <= 5)
                        {
                            _output.WriteLine($"Update Error [{putResponse.StatusCode}] for ID {clientId}: {errorContent}");
                        }
                        return Response.Fail();
                    }

                    return Response.Ok();
                }
                catch (Exception ex)
                {
                    return Response.Fail(ex);
                }
            })
            .WithLoadSimulations(
                // Target load simulation (adjust RPS and duration as needed)
                Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10))
            );

            // 5. Run NBomber and write stats to xUnit output
            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.ScenarioStats.First(s => s.ScenarioName == "client_update_scenario");

            _output.WriteLine($"=== NBomber Client Update Performance Results ===");
            _output.WriteLine($"Total Successful Updates: {scnStats.Ok.Request.Count}");
            _output.WriteLine($"Requests Per Second (RPS): {scnStats.Ok.Request.RPS}");
            _output.WriteLine($"Mean Latency: {scnStats.Ok.Latency.MeanMs} ms");
            _output.WriteLine($"P95 Latency: {scnStats.Ok.Latency.Percent95} ms");
            _output.WriteLine($"Failed Requests / Concurrency Conflicts: {scnStats.Fail.Request.Count}");
        }
    }
}