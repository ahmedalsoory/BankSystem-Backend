using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Checkbook;
using Dapper;
using Shared.Enums.Checkbook;
using Repositories.Queries;
using RepositoryContracts.Checkbook;
using DTOs.CheckbookApplications;

namespace Repositories.Banking
{
    public class CheckbookRepository :BaseRepository , ICheckbookWriteRepository,
        ICheckbookReadTransactionRepository
    {
        public CheckbookRepository(IDbContextScope dbContextScope, Context error
           , IAuditTracker auditTracker) : base(dbContextScope, error, auditTracker)
        {

        }

        public async Task<OperationResult<int>> Add(CheckbookAddRequest request)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();

            await base.BeginTransactionAsync();

            int lastEndNumber = await connection.ExecuteScalarAsync<int>(
                Query.Checkbook.getLastCheck,
                new { request.AccountID },
                base.CurrentTransaction
            );

            int beginCheckNumber = lastEndNumber + 1;
            int endCheckNumber = beginCheckNumber + request.NumberOfLeaves - 1;

            int checkbookId = await connection.ExecuteScalarAsync<int>(
                Query.Checkbook.Insert,
                new
                {
                    request.AccountID,
                    request.ApplicationID,
                    BeginCheckNumber = beginCheckNumber,
                    EndCheckNumber = endCheckNumber,
                    Status = CheckbookStatus.PendingApproval,
                },
                base.CurrentTransaction
            );

            return checkbookId == 0 ? OperationResult<int>.Failure("add check book filar")
                : OperationResult<int>.Ok(checkbookId);

        }

        public async Task<CheckbookApplicationDetails> GetDetailsForIssueFirstTime(int applicationId)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();
           

            var result = await connection.QueryFirstOrDefaultAsync<CheckbookApplicationDetails>(
                Query.Checkbook.GetDetailsForIssueFirstTime,
                new { ApplicationID = applicationId },transaction:base.CurrentTransaction
            );

            return result;
        }
    

        public async  Task<CheckbookApplicationDetails> GetDetailsForReplacementOrRenew(int applicationId)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            var result = await connection.QueryFirstOrDefaultAsync<CheckbookApplicationDetails>(
                Query.Checkbook.GetDetailsForReplacementOrRenew,
                new { ApplicationID = applicationId }, transaction: base.CurrentTransaction
            );

            return result;
        }
    

        public async Task<OperationResult> UpdateStatus(int id, CheckbookStatus newStatus)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();
            int rowsAffected = await connection.ExecuteAsync(
                Query.Checkbook.UpdateStatus,
                new
                {
                    CheckbookID = id,
                    Status = (byte)newStatus
                },
                base.CurrentTransaction
            );

            if (rowsAffected == 0)
            {
                return OperationResult.Failure("Checkbook record not found or status update failed.");
            }

            _auditTracker?.AddEntry(
                tableName: "Checkbooks",
                recordId: id.ToString(),
                operationType: "Update",
                userId: "2"
            );

            return OperationResult.Ok();
        }

   
    }
}
