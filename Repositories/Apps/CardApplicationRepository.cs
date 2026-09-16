using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.CardApplication;
using Dapper;
using Repositories.Queries;
using RepositoryContracts.CardApplication;

namespace Repositories.Apps
{
    public class CardApplicationRepository : BaseRepository, ICardApplicationWriteRepository
    {

        public CardApplicationRepository(IDbContextScope dbContextScope, Context error
            , IAuditTracker auditTracker) : base(dbContextScope, error, auditTracker:auditTracker)
        {

        }

        public async Task<OperationResult<int>> AddNewCardApplicationAsync(CardApplicationAddRequest request, int applicationId)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();



            int newId = await connection.ExecuteScalarAsync<int>(Query.CardApplication.InsertNewCardApplication, new
            {
                ApplicationId = applicationId,
                request.AccountID,
                request.IsInternationalEnabled,
                request.CardTypeID
            }, transaction: CurrentTransaction);

            if (newId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
             tableName: "CardApplication",
             recordId: newId.ToString(),
             operationType: "Insert",
             userId: "2");


            return OperationResult<int>.Ok(newId);
        }
  
        public async Task<OperationResult<int>> AddRenewApplicationAsync(CardRenewApplicationAddRequest request, int applicationId)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();



            int newId = await connection.ExecuteScalarAsync<int>(Query.CardApplication.InsertRenewApplication, new
            {
                ApplicationId = applicationId,
                request.IsInternationalEnabled,
                OldCard = request.oldCardId
            }, transaction: CurrentTransaction);
            if (newId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
             tableName: "CardApplication",
             recordId: newId.ToString(),
             operationType: "Insert",
             userId: "2");

            return OperationResult<int>.Ok(newId);
        }

        // 2. For Replacements or Renewals (Zero joins, inherits directly from the old card)
        public async Task<OperationResult<int>> AddReplacementApplicationAsync(CardReplaceApplicationAddRequest request, int applicationId)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();



            int newId = await connection.ExecuteScalarAsync<int>(Query.CardApplication.InsertReplacementApplication, new
            {
                ApplicationId = applicationId,
                request.IsInternationalEnabled,
                OldCardId = request.oldCardId
            }, transaction: CurrentTransaction);

            if (newId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
            tableName: "CardApplication",
            recordId: newId.ToString(),
            operationType: "Insert",
            userId: "2");

            return OperationResult<int>.Ok(newId);
        }


    }
}
