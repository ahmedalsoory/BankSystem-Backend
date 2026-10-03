using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Helper;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;

namespace Tests.AccountOpeningOrchestrator.Integration
{
    public partial class AccountOpeningOrchestratorWorkflowTests
    {
        [Fact]
        public async Task ProcessStatusVerification_WhenAllStepsCompletedDynamically_ShouldAutoCreateAccountAndDeposit()
        {
         

            var result = await new PersonBuilder().WithName("Ahm2ed", "ali")
                .AsClient()
                .ExecuteFullWorkflowAsync(_scopedServiceProvider);

            // 2. Assert
            result.Success.Should().BeTrue($"because workflow execution failed: {result.ErrorMessage}");

            MarkTestAsSuccessful();
        }
    }
}
