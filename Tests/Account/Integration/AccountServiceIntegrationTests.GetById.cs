using FluentAssertions;
using System.Threading.Tasks;
using Tests.Account.helper;

namespace Tests.Account.Integration
{
    public partial class AccountServiceIntegrationTests
    {
        [Fact]
        public async Task GetByIdAsync_WhenAccountExists_ShouldReturnAccountDetails()
        {
            // Arrange - Assuming account ID 1 is seeded in your test database
            int? existingAccountId = await TestAccountDataHelper.getAccountID(_connectionProvider) ;

            // Act
            var result = await AccountReadService.GetByIdAsync(existingAccountId.Value);

            // Assert
            result.Should().NotBeNull();
            result!.AccountID.Should().Be(existingAccountId);

            MarkTestAsSuccessful();
        }

        [Fact]
        public async Task GetByIdAsync_WhenAccountDoesNotExist_ShouldReturnNull()
        {
            // Arrange - Use an ID that definitely doesn't exist
            int nonExistentAccountId = -999;

            // Act
            var result = await AccountReadService.GetByIdAsync(nonExistentAccountId);

            // Assert
            result.Should().BeNull();

            MarkTestAsSuccessful();
        }
    }
}