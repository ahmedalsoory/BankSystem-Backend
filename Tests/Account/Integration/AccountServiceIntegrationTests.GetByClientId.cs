using FluentAssertions;
using System.Threading.Tasks;
using Tests.CardApplication.helper;
using Tests.Client.helper;

namespace Tests.Account.Integration
{
    public partial class AccountServiceIntegrationTests
    {
        [Fact]
        public async Task GetByClientIdAsync_WhenClientHasAccounts_ShouldReturnList()
        {
            // Arrange
            int clientId = await TestClientDataHelper.GetLatestClientIdAsync(); // Seeded client with accounts

            // Act
            var results = await AccountReadService.GetByClientIdAsync(clientId);

            // Assert
            results.Should().NotBeNull();
            results.Should().NotBeEmpty();
            MarkTestAsSuccessful();
        }

        [Fact]
        public async Task GetByClientIdAsync_WhenClientHasNoAccounts_ShouldReturnEmptyList()
        {
            // Arrange - Client ID with no accounts
            int emptyClientId = -123;

            // Act
            var results = await AccountReadService.GetByClientIdAsync(emptyClientId);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeEmpty();
            MarkTestAsSuccessful();
        }
    }
}