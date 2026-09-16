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

namespace Repositories.Banking
{
    public class CheckbookRepository :BaseRepository
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

            var getLastCheckQuery = @"
            SELECT ISNULL(MAX(EndCheckNumber), 100000) 
            FROM Banking.Checkbooks 
            WHERE AccountID = @AccountID";

            int lastEndNumber = await connection.ExecuteScalarAsync<int>(
                getLastCheckQuery,
                new { request.AccountID },
                base.CurrentTransaction
            );

            int beginCheckNumber = lastEndNumber + 1;
            int endCheckNumber = beginCheckNumber + request.NumberOfLeaves - 1;

            // 2. Insert the Checkbook record and get the new CheckbookID
            var insertCheckbookQuery = @"
            INSERT INTO Banking.Checkbooks (AccountID, ApplicationID, BeginCheckNumber, EndCheckNumber, Status, CreatedDate)
            VALUES (@AccountID, @ApplicationID, @BeginCheckNumber, @EndCheckNumber, @Status, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int checkbookId = await connection.ExecuteScalarAsync<int>(
                insertCheckbookQuery,
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
