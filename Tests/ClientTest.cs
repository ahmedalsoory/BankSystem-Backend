using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;

namespace Tests
{
    public class ClientTest : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        private readonly IClientWriteService _clientWriteService;

        public ClientTest(IntegrationTestFixture fixture) : base(fixture)
        {
            var scope = _serviceProvider.CreateScope();
            _clientWriteService = scope.ServiceProvider.GetRequiredService<IClientWriteService>();
        }

        [Fact]
        public async Task CreateAsync_WhenBuildingClient_ShouldSucceed()
        {
            // Arrange & Act
            var result = await new PersonBuilder()
                .WithName("Khaled", "Youssef")
                .AsClient()
                .WithRiskLevel(2)
                .WithIsActive(true)
                .BuildAsync(_clientWriteService);

            // Assert success first
            result.Success.Should().BeTrue($"because: {string.Join(", ", result.Errors)}");
            result.Data.Should().BeGreaterThan(0);

            // Mark the test as successful so the transaction pipeline commits automatically on dispose
            MarkTestAsSuccessful();
        }
    }
}
