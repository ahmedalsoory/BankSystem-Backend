using FluentAssertions;
using System.Threading.Tasks;

namespace Tests.AccountApplications.Integration
{
    public partial class AccountApplicationServiceIntegrationTests
    {
        [Fact]
        public async Task GetByIdAsync_WhenApplicationExists_ShouldReturnDetails()
        {
            // Arrange - Assuming application ID 1 is seeded in your test database
            int existingId = 1;

            
            // Act
            var result = await ReadService.GetByIdAsync(existingId);

            // Assert
            result.Should().NotBeNull();
            result.AccountID.Should().Be(existingId);
        }

        [Fact]
        public async Task GetByIdAsync_WhenApplicationDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            int nonExistentId = -999;

            // Act
            var result = await ReadService.GetByIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }
    }
}