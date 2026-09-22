using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Helper;

namespace Tests.AccountOpeningOrchestrator.Integration
{
    public partial class AccountOpeningOrchestratorWorkflowTests
    {
        [Fact]
        public async Task ProcessStatusVerification_WhenAllStepsCompletedDynamically_ShouldAutoCreateAccountAndDeposit()
        {
            // 1. Arrange & Act: Execute the full dynamic workflow using the helper
            var result = await AccountWorkflowExecutionHelper.ExecuteWorkflowAsync(
                _serviceProvider,
                "Ayman",
                "Salem",
                "ayman.salem@bank.com"
            );

            // 2. Assert
            result.Success.Should().BeTrue($"because workflow execution failed: {result.Message}");

            _dbContextScope.Commit();
            MarkTestAsSuccessful();
        }
    }
}
