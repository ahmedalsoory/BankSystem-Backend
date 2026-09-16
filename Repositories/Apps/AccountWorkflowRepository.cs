using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using RepositoryContracts.AccountWorkflowRepository;
using DTOs.AccountOpeningWorkflow;
using DTOs.interfaces;
using DTOs.AccountOpeningWorkflow.interfaces;
using Repositories.Queries;

namespace Repositories.Apps
{
    public class AccountWorkflowRepository : BaseRepository, IAccountWorkflowWriteRepository, IAccountWorkflowReadRepository
    {
       
        public AccountWorkflowRepository(
            IDbContextScope dbContextScope,
            Context error, IAuditTracker auditTracker,
            IOptions<DbSettings> options) : base(dbContextScope, error, auditTracker, options)
        {

        }

        public async Task<OperationResult> AddWorkflowStepsAsync(WorkflowInitializationRequest request)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int rowEffect = await connection.ExecuteAsync(
                Query.AccountWorkflow.AddWorkflowSteps, request, CurrentTransaction);

            if(rowEffect == 0)
                return OperationResult.Failure("No workflow steps found.");

            _auditTracker?.AddEntry(
            tableName: "AccountWorkflow",
            recordId: request.ApplicationID.ToString(),
            operationType: "Insert",
            userId: "2");


            return OperationResult.Ok();
        }

        public async Task<OperationResult> CompleteStepAsync(WorkflowCompleteStatusUpdateRequest request)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int rowsAffected = await connection.ExecuteAsync(
                Query.AccountWorkflow.CompleteStep, request, CurrentTransaction);

            if(rowsAffected == 0)
                return OperationResult.Failure("Step already completed or invalid ID.");

            _auditTracker?.AddEntry(
             tableName: "AccountWorkflow",
             recordId: request.ApplicationID.ToString(),
             operationType: "Update",
             userId: "2");

            return OperationResult.Ok();
        }

        public async Task<OperationResult> IsAllStepCompleted(int ApplicationID)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            // No need for a transaction for a read-only check
            await BeginTransactionAsync();
            // ExecuteScalar returns the count. If > 0, steps are still pending.
            int pendingSteps = await connection.ExecuteScalarAsync<int>(
                Query.AccountWorkflow.CheckIncompleteSteps, new { ApplicationID }, CurrentTransaction);

            return pendingSteps == 0 ? OperationResult.Ok() : OperationResult.Failure("Workflow not yet completed.");
        }
    }
}
