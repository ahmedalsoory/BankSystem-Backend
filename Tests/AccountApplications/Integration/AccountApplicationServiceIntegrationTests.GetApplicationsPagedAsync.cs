using DTOs.AccountApplications;
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
        public async Task GetApplicationsPagedAsync_WithValidRequest_ShouldReturnPagedResults()
        {
            // Arrange
            var request = new AccountApplicationPagedRequest
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await ReadService.GetApplicationsPagedAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            //result.to.Should().Be(1);
        }
    }
}
