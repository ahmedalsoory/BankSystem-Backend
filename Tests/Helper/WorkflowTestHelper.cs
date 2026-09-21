using Dapper;
using Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Helper
{
    public static class WorkflowTestHelper
    {
        public static async Task<IEnumerable<WorkflowStepDto>> GetStepsByApplicationIdAsync(
            int applicationId,
            IDbConnectionProvider connectionProvider,
            IDbContextScope dbContextScope = null) // Accept optional scope to bind transaction
        {
            var connection = await connectionProvider.GetConnectionAsync();
            await connectionProvider.BeginTransactionAsync();
          
            var sql = @"
                SELECT StepID, StepName
                FROM Apps.ApplicationWorkflowSteps 
                WHERE ApplicationID = @ApplicationID";

            // If your IDbContextScope exposes the active transaction, pass it to Dapper's command definition
            // e.g., command: new CommandDefinition(sql, new { ApplicationID = applicationId }, transaction: dbContextScope?.CurrentTransaction)

            return await connection.QueryAsync<WorkflowStepDto>(
                sql,
                new { ApplicationID = applicationId }, connectionProvider.CurrentTransaction
            );
        }
    }

    public class WorkflowStepDto
    {
        public int StepID { get; set; }
        public string StepName { get; set; }
    
    }
}