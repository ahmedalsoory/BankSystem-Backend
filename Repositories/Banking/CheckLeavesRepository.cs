using Dapper;
using DTOs.Checkbook;
using DTOs.CheckLeaves;
using Repositories.Queries;
using RepositoryContracts.CheckLeaves;
using Shared;
using Shared.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Repositories.Banking
{
    public class CheckLeavesRepository : BaseRepository, ICheckLeavesWriteRepository
    {
        public CheckLeavesRepository(
            IDbContextScope dbContextScope,
            Context error,
            IAuditTracker auditTracker) : base(dbContextScope, error, auditTracker)
        {
        }

        public async Task<OperationResult> AddRangeAsync(AddCheckLeafs request)
        {
            base.SetAction();

            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

          

            int rowsAffected = await connection.ExecuteAsync(
                Query.CheckLeaves.AddRangeAsync,
                new
                {
                    CheckbookID = request.CheckbookID,
                    AccountID = request.AccountID,
                    BeginCheckNumber = request.BeginCheckNumber,
                    NumberOfLeaves = request.NumberOfLeaves,
                    Status = request.Status
                },
                base.CurrentTransaction
            );

            if (rowsAffected == 0)
            {
                return OperationResult.Failure("Failed to insert check leaves batch.");
            }

            _auditTracker?.AddEntry(
                tableName: "CheckLeaves",
                recordId: request.CheckbookID.ToString(),
                operationType: "Insert",
                userId: "2"
            );

            return OperationResult.Ok();
        }
        public async Task<OperationResult> DisableUnusedLeavesAsync(DisableUnusedLeaves request)
        {
          
            base.SetAction();
            var connection = await base.GetConnectionAsync();

            await base.BeginTransactionAsync();

            int rowEffect= await connection.ExecuteAsync(Query.CheckLeaves.DisableUnusedLeavesAsync, new
            {
                OldCheckbookID =request.OldCheckbookId,
                DisabledStatus = (byte) request.DisabledStatus,
                ActiveStatus = (byte) request.ActiveStatus
            }, base.CurrentTransaction);

            _auditTracker?.AddEntry(
              tableName: "CheckLeaves",
              recordId: request.OldCheckbookId.ToString(),
              operationType: "Update status",
              userId: "2"
          );

            if (rowEffect == 0)
                return OperationResult.Failure("Update check leaf faild");
            return OperationResult.Ok();
        }
    }
}