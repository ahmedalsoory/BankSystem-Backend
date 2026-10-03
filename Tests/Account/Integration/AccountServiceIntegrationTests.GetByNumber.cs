using FluentAssertions;
using System.Threading.Tasks;
using Tests.Account.helper;

namespace Tests.Account.Integration
{
    public partial class AccountServiceIntegrationTests
    {
        [Fact]
        public async Task GetByNumberAsync_WhenNumberExists_ShouldReturnAccount()
        {
            // Arrange
            string? accountNumber = await TestAccountDataHelper.getAccountNumber(_dbContextScope);

            // Act
            var result = await AccountReadService.GetByNumberAsync(accountNumber);

            // Assert
            result.Should().NotBeNull();
            result!.AccountNumber.Should().Be(accountNumber);

            MarkTestAsSuccessful();
        }

        [Fact]
        public async Task GetByNumberAsync_WhenNumberDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            string fakeAccountNumber = "INVALID-ACC-99999";

            // Act
            var result = await AccountReadService.GetByNumberAsync(fakeAccountNumber);

            // Assert
            result.Should().BeNull();


            MarkTestAsSuccessful();
        }
    }
}