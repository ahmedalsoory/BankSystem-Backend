using Dapper;
using Shared.Interfaces;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs.interfaces;
using DTOs.AccountOpeningWorkflow.interfaces;
using Repositories.Queries.QueriesValiditon;

namespace Repositories.Validations
{
    public class WorkflowValidationRepository : BaseRepository, 
        IValidationService<IWorkflowValidationDTO>
    {
        public WorkflowValidationRepository(IDbConnectionProvider provider, Context error)
            : base(provider, error) { }

        public async Task<List<string>> ValidateAsync(IWorkflowValidationDTO dto, int? id = null)
        {
            var errors = new List<string>();

            // 1. Get the target index (Configuration Lookup)
            int targetIndex = await GetStepOrderIndex(dto);

            // 2. Validate current state against that index (State Validation)
            if (!await IsSequenceValid(dto, targetIndex, errors))
                return errors;

            return errors;
        }

        private async Task<int> GetStepOrderIndex(IWorkflowValidationDTO dto)
        {
     

            var connection = await base.GetConnectionAsync();
            await BeginTransactionAsync(); 
            return await connection.ExecuteScalarAsync<int>(QueryValidtion.WorkflowValidation.GetStepOrderIndex,
                new { dto.ApplicationID, dto.StepName },
                base.CurrentTransaction);
        }

        private async Task<bool> IsSequenceValid(IWorkflowValidationDTO dto, int targetIndex, List<string> errors)
        {
  
            var connection = await base.GetConnectionAsync();
            await BeginTransactionAsync();
            int blockingSteps = await connection.ExecuteScalarAsync<int>(QueryValidtion.WorkflowValidation.CheckForBlockingSteps,
                new { dto.ApplicationID, TargetIndex = targetIndex },
                base.CurrentTransaction);

            if (blockingSteps > 0)
            {
                errors.Add($"Validation Failed: Cannot complete step '{dto.StepName}' because a previous workflow step is still pending.");
                return false;
            }
            return true;
        }
    }
}