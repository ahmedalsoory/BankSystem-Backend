using DTOs.Client;
using FluentAssertions;
using ServiceContract.Client;
using System.Threading.Tasks;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;
using Xunit;
using Tests.Client.helper;
using System;

namespace Tests.Client.Integration
{
    public partial class ClientIntegrationTest
    {
        [Fact]
        public async Task UpdateAsync_WhenRowVersionIsStale_ShouldFailWithConcurrencyError()
        {
            Random random = new Random();
        int personId = random.Next(400058, 973015);
            // 2. Fetch the client to get valid, fresh RowVersions

            var clientDetail = null as ClientDetailDto;
            clientDetail = await TestClientDataHelper.GetClientDetailDirectlyAsync(personId,
               _connectionProvider);
            while (clientDetail == null)
            {
                 personId = random.Next(400058, 973015);
                clientDetail = await TestClientDataHelper.GetClientDetailDirectlyAsync(personId,
               _connectionProvider);

            }
            clientDetail.Should().NotBeNull();

            // 3. Prepare the update request with current tokens
            var updateRequest = new ClientUpdateRequest
            {
                Id = personId,
                FirstName = "MohamedUpdated",
                LastName = "AliUpdated",
                NationalId = clientDetail!.NationalId,
                Email = clientDetail.Email,
                Phone = clientDetail.Phone,
                BirthDate = clientDetail.BirthDate,
                Gendor = clientDetail.Gendor,
                RiskLevel = 2,
                IsActive = true,
                PersonRowVersion = clientDetail.PersonVersion,
                ClientRowVersion = clientDetail.ClientVersion,
            };

            // 4. Act Part A: First update succeeds and changes the RowVersions in the database
            var firstUpdateResult = await _clientWriteService.UpdateAsync(updateRequest, null);
            firstUpdateResult.Success.Should().BeTrue();

            // 5. Act Part B: Try updating *again* using the exact same stale request tokens
            var staleUpdateResult = await _clientWriteService.UpdateAsync(updateRequest, null);

            // 6. Assert: The second update must fail due to the RowVersion mismatch
            staleUpdateResult.Success.Should().BeFalse();
            staleUpdateResult.Errors.Should().NotBeEmpty();

            MarkTestAsSuccessful();
        }
    }
}