using DTOs.Client;
using FluentAssertions;
using Shared.Enums.Client;
using Shared.Enums;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Client.Integration
{
    public partial class ClientIntegrationTest
    {
        [Fact]
        public async Task GetClientsAsync_WithPaginationAndFilter_ShouldReturnFilteredAndSortedList()
        {
            // 1. Arrange
            var request = new ClientPagedRequest
            {
                PageNumber = 1,
                pageSize = 10,
                SortBy = SortedBy_Client.Name,
                Direction = Direction.ACS,
                FilterBy = Filter_Client.Name,
                FilterValue = "Ahmed"
            };

            // 2. Act
            var pagedResult = await _clientReadService.GetClientsAsync(request);

            // 3. Materialize to a List to verify sorting and filtering
            var clientsList = pagedResult.Data.ToList();

            // 4. Assert
            pagedResult.Should().NotBeNull();
            clientsList.Should().NotBeNull();

            // Verify filter worked
            foreach (var client in clientsList)
            {
                client.FirstName.ToLower().Should().Contain("ahmed");
            }

          
            clientsList.Should().BeInAscendingOrder(c => c.FirstName, StringComparer.OrdinalIgnoreCase);

            MarkTestAsSuccessful();
        }
    }
}