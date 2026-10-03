using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using Tests.Client.helper;
using Xunit;

namespace Tests.Client.Integration
{
    public partial class ClientIntegrationTest
    {
        [Fact]
        public async Task GetByClientNumberAsync_WhenClientExists_ShouldReturnClientDetails()
        {
            // 1. Fetch the exact ClientNumber directly from the database
            string? clientNumber = await TestClientDataHelper.GetLatestClientNumberAsync();
            clientNumber.Should().NotBeNullOrWhiteSpace();

            // 2. Act: Call the read service using the verified ClientNumber
            var clientDetail = await _clientReadService.GetByAccountNumberAsync(clientNumber);

            // 3. Assert: Verify the retrieved data matches expectations
            clientDetail.Should().NotBeNull();
            clientDetail!.ClientNumber.Should().Be(clientNumber);
            clientDetail.FirstName.Should().NotBeNullOrWhiteSpace();

         
        }
    }
}