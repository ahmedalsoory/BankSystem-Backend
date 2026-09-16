using DTOs.AccountOpeningApplication;
using DTOs.AccountOpeningWorkflow;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountWorkflowRepository
{
    public interface IAccountWorkflowWriteRepository
    {
        Task<OperationResult> AddWorkflowStepsAsync(WorkflowInitializationRequest request);


        Task<OperationResult> CompleteStepAsync(WorkflowCompleteStatusUpdateRequest request);
    }
}
