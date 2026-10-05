using DTOs.AccountApplications;
using FluentAssertions;
using Shared.Enums.AccountApplications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.CardApplication.helper;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;

namespace Tests.AccountApplications.Orchestrators.Integration
{
    public partial class AccountApplicationUpdateStatusOrchestratorIntegrationTests
    {
        [Fact]
        public async Task UpdateStatus_WhenStatusIsApproved_CardApplicationIsuee()
        {


            var workflowResult = await new PersonBuilder()
               .WithName("Khaled", "Youssef")
               //  .WithEmail("k1h00al22ed@test.com")
               .AsClient()
               .ExecuteFullWorkflowAsync(

                   _scopedServiceProvider // Passed directly from base class
               );

            workflowResult.Success.Should().BeTrue($"because: {workflowResult.ErrorMessage}");

            // 2. Use the captured Account ID safely
            int accountId = await TestCardApplicationDataHelper.getLastAccountId(_dbContextScope);

            var cardResult = await new AccountApplicationBuilder()
                .ForAccount(accountId)
                .WithNotes("First-time local visa issuance")
                .AsCardApplication()
                .WithCardType(1)
                .BuildAsync(_cardApplicationWriteService);



            // Arrange - Use an existing pending application ID from your test DB
            var request = new AccountApplicationStatusUpdateRequest
            {
                ApplicationId = cardResult.Data, // Seeded application ID
                NewStatus = ApplicationStatus.Approved, // Not Approved
                Notes = "test confirm"
            };

            // Act
            var result = await Orchestrator.UpdateStatus(request);

            // Assert
            result.Success.Should().BeTrue();
            MarkTestAsSuccessful();
        }
    }
}
