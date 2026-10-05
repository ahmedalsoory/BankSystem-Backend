using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.CardApplication;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.CardApplication.helper;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;

namespace Tests.CardApplication.Integration
{
    public partial class CardApplicationIntegrationTest
    {
        [Fact]
        public async Task CreateCardApplication_WhenFirstTime_ShouldSucceed()
        {
            // 1. Execute the workflow using the shared class-level services 
            // (They automatically share the exact same active transaction pipe!)
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

            // 3. Assert
            cardResult.Success.Should().BeTrue($"because: {string.Join(", ", cardResult.Errors)}");
            cardResult.Data.Should().BeGreaterThan(0);

            // 4. Signal success so the base class commits the transaction automatically on disposal!
            MarkTestAsSuccessful();
        }
    
    }
}
