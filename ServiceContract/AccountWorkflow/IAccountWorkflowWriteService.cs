using DTOs.AccountOpeningWorkflow;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountWorkflow
{
    public interface IAccountWorkflowWriteService
    {

        Task<OperationResult> AddWorkflowStepsAsync(WorkflowInitializationRequest request);


        Task<OperationResult> CompleteStepAsync(WorkflowCompleteStatusUpdateRequest request);
    }
}
