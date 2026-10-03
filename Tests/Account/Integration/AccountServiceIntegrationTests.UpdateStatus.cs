using FluentAssertions;
using DTOs.Account;
using System.Threading.Tasks;
using Tests.Account.helper;

namespace Tests.Account.Integration
{
    public partial class AccountServiceIntegrationTests
    {
        [Fact]
        public async Task UpdateStatus_WhenValidRequest_ShouldSucceed()
        {
            var data = await TestAccountDataHelper.getAccountData(_connectionProvider);
            // Arrange
            var updateRequest = new AccountUpdateStatusRequest
            {
                AccountID = data.AccountID,
                Status = (data.Status == 1) ? Shared.Enums.Account.enAccountStatus.Active :
               (Shared.Enums.Account.enAccountStatus)(data.Status - 1),
                RowVersion = data.RowVersion
               
            };

            // Act
            var result = await AccountWriteService.UpdateStatus(updateRequest);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            MarkTestAsSuccessful();
        }

        [Fact]
        public async Task UpdateStatus_WhenAccountNotFound_ShouldReturnFailure()
        {
            // Arrange
            var updateRequest = new AccountUpdateStatusRequest
            {
                AccountID = -999, // Non-existent account
                Status = Shared.Enums.Account.enAccountStatus.Active
            };

            // Act
            var result = await AccountWriteService.UpdateStatus(updateRequest);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();

            MarkTestAsSuccessful();
        }
    }
}