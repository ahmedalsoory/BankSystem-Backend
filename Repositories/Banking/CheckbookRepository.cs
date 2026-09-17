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

namespace Repositories.Banking
{
    public class CheckbookRepository :BaseRepository , ICheckbookWriteRepository
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


    }
}
