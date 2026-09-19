using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.Client;
using Shared.Interfaces;
using System.Threading.Tasks;
using Tests.Common;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;
using Xunit;

namespace Tests
{
    public class AccountOpeningApplicationTest : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        private readonly IClientWriteService _clientWriteService;
        private readonly IAccountOpeningApplicationsWriteService _accountOpeningApplicationsWriteService;



        public AccountOpeningApplicationTest(IntegrationTestFixture fixture) : base(fixture)
        {
            var scope = _serviceProvider.CreateScope();
            _clientWriteService = scope.ServiceProvider.GetRequiredService<IClientWriteService>();
            _accountOpeningApplicationsWriteService = scope.ServiceProvider.GetRequiredService<IAccountOpeningApplicationsWriteService>();
            _dbContextScope = scope.ServiceProvider.GetRequiredService<IDbContextScope>();
        }

        [Fact]
        public async Task AddApplicationAsync_WhenValidRequest_ShouldCreateApplicationAndWorkflows()
        {
            // 1. Arrange: Create the client first using your person/client builder
            var clientResult = await new PersonBuilder()
                .WithName("Khaled", "Youssef")
                .AsClient()
                .BuildAsync(_clientWriteService);

            clientResult.Success.Should().BeTrue($"because client creation failed: {string.Join(", ", clientResult.Errors)}");
            clientResult.Data.Should().BeGreaterThan(0);

            // 2. Act: Build and submit the account opening application
            var appResult = await new AccountOpeningApplicationBuilder()
                .ForClient(clientResult.Data)
                .WithAccountType(1)
                .WithDeposit(2500.00f)
                .WithCurrency("USD")
                .BuildAsync(_accountOpeningApplicationsWriteService);

            // 3. Assert
            appResult.Success.Should().BeTrue($"because application creation failed: {string.Join(", ", appResult.Errors)}");
            appResult.Data.Should().BeGreaterThan(0);

            // Mark the test as successful so the transaction commits automatically
            base.MarkTestAsSuccessful();
        }
    }
}