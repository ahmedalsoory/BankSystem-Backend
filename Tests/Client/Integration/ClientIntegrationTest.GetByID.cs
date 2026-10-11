using FluentAssertions;
using System.Threading.Tasks;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;
using Tests.Client.helper;
using Xunit;

namespace Tests.Client.Integration
{
    public partial class ClientIntegrationTest
    {
        [Fact]
        public async Task GetByClientIDAsync_WhenClientExists_ShouldReturnClientDetails()
        {
            // 1. Arrange: Create a client using the builder
           

            // 2. Fetch the ID directly using your SQL query strategy
            int clientId = await TestClientDataHelper.GetLatestClientIdAsync();
            clientId.Should().BeGreaterThan(0);

            // 3. Act: Call the read service using that verified database ID
            var clientDetail = await _clientReadService.GetByClientIDAsync(clientId);

            // 4. Assert: Verify the retrieved data matches what was created
            clientDetail.Should().NotBeNull();

        }
    }
}