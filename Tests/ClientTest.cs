using FluentAssertions;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;

namespace Tests
{
    public class ClientTest
    {
        private readonly IClientWriteService _clientWriteService;
        private readonly IDbContextScope _dbContextScope;

        public ClientTest(IClientWriteService clientWriteService, IDbContextScope dbContextScope)
        {
            _clientWriteService = clientWriteService;
            _dbContextScope = dbContextScope;
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

            // Explicitly commit so it persists in the test database for verification
            _dbContextScope.Commit();
        }
    }
}
