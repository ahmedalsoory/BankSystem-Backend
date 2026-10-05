using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.AccountApplications.Integration
{
    public partial class AccountApplicationServiceIntegrationTests
    {
        [Fact]
        public async Task GetApplicationTypeByIdAsync_WhenTypeExists_ShouldReturnApplicationType()
        {
            // Arrange - Assuming type ID 1 exists
            int existingTypeId = 1;

            // Act
            var result = await ReadTransactionService.GetApplicationTypeByIdAsync(existingTypeId);

            // Assert
            result.Should().BeDefined();
        }

        [Fact]
        public async Task GetApplicationTypeByIdAsync_WhenTypeDoesNotExist_ShouldHandleGracefully()
        {
            // Arrange
            int invalidTypeId = 100;

            // Act
            var result = await ReadTransactionService.GetApplicationTypeByIdAsync(invalidTypeId);

            // Assert - Depending on casting/enums, it may return default or handle as 0/null
            // Adjust assertion according to your enum default representation if needed
            result.Should().BeNull();
        }
    }
}