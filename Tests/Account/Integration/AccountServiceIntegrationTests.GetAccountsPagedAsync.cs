using FluentAssertions;
using DTOs.Account;
using Shared.Enums; // For Direction enum if needed
using Shared.Enums.Account; // For SortedBy_Account and Filter_Account enums
using System.Threading.Tasks;
using Tests.Account.helper;

namespace Tests.Account.Integration
{
    public partial class AccountServiceIntegrationTests
    {
        [Fact]
        public async Task GetAccountsPagedAsync_WithDefaultValues_ShouldReturnPagedResults()
        {
            // Arrange
            var request = new AccountPagedRequest
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await AccountReadService.GetAccountsPagedAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
          
            result.Data.Count().Should().Be(10);

            MarkTestAsSuccessful();
        }

        [Fact]
        public async Task GetAccountsPagedAsync_WithSortingAndFiltering_ShouldReturnFilteredResults()
        {
            // Arrange - Test custom sorting and optional filters (e.g., filtering by Account Number or Status)
            var request = new AccountPagedRequest
            {
                PageNumber = 1,
                PageSize = 5,
                SortBy = SortedBy_Account.CreatedDate,
                Direction = Direction.DESC,
                FilterBy = Filter_Account.AccountNumber, // Adjust based on your available enum fields
                FilterValue = "ACC" // Or a known test token/value
            };

            // Act
            var result = await AccountReadService.GetAccountsPagedAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();

            MarkTestAsSuccessful();

        }
        [Fact]
        public async Task GetAccountsPagedAsync_WhenFilteredByAccountNumber_AllResultsShouldMatchFilter()
        {
            // Arrange - Get a real account number dynamically using your transaction-safe helper
            string targetAccountNumber = await TestAccountDataHelper.getAccountNumber(_connectionProvider);

            var request = new AccountPagedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                FilterBy = Filter_Account.AccountNumber,
                FilterValue = targetAccountNumber
            };

            // Act
            var result = await AccountReadService.GetAccountsPagedAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();

            // Verify strict filtering: Every returned item must contain or match the filter value
            foreach (var item in result.Data)
            {
                item.AccountNumber.Should().Contain(targetAccountNumber);
            }

            MarkTestAsSuccessful();

        }

        [Fact]
        public async Task GetAccountsPagedAsync_WhenSortedByBalanceDescending_ShouldBeOrderedCorrectly()
        {
            // Arrange
            var request = new AccountPagedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = SortedBy_Account.Balance,
                Direction = Direction.DESC
            };

            // Act
            var result = await AccountReadService.GetAccountsPagedAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();

            if (result.Data.Count() > 1)
            {
                // Verify strict sorting: Ensure each item's balance is greater than or equal to the next
                var balances = result.Data.Select(x => x.Balance).ToList();
                balances.Should().BeInDescendingOrder();
                // Note: If using standard LINQ without a helper extension, you can also write:
                // var balances = result.Items.Select(x => x.Balance).ToList();
                // balances.Should().BeInDescendingOrder();
            }

            MarkTestAsSuccessful();

        }
    }
}