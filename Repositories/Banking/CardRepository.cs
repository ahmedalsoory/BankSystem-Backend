using DTOs.Account.interfaces;
using DTOs.interfaces;
using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Card;
using Dapper;

using Repositories.Queries;
using static System.Net.Mime.MediaTypeNames;
using RepositoryContracts.Card;
using Shared.Enums.Card;

namespace Repositories.Banking
{
    public class CardRepository:BaseRepository, ICardReadTransactionRepository, ICardWriteRepository
    {
        public CardRepository(IDbContextScope dbContextScope, Context error
            , IAuditTracker auditTracker) : base(dbContextScope, error, auditTracker)
        {
            
        }
        public async Task<CardApplicationDetails> GetApplicationDetailsForCardCreationAsync(int applicationId)
        {
            SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            var result = await connection.QueryFirstOrDefaultAsync<CardApplicationDetails>(
        Query.Card.GetDeatils,
        new { ApplicationID = applicationId },
        transaction: base.CurrentTransaction // Adjust this property name to match your base class transaction object
    );

            return result;

        }
        public async Task<OperationResult<int>> AddCard(CardAddRequest request)
        {
            SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            int newCardId = await connection.QuerySingleAsync<int>(Query.Card.Insert, request
                ,base.CurrentTransaction);

            if (newCardId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
             tableName: "Card",
             recordId: newCardId.ToString(),
             operationType: "Insert",
             userId: "2");


            return  OperationResult<int>.Ok( newCardId);
        }

        public async Task<OperationResult> UpdateStatus(int cardId, CardStatus newStatus)
        {
            SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();
            int rowsAffected = await connection.ExecuteAsync(
            Query.Card.UpdateStatus,
            new { cardId, newStatus = (byte)newStatus },
            base.CurrentTransaction
        );

            if (rowsAffected == 0)
            {
              
                return OperationResult.Failure("Card not found or status update failed.");
            }

            // Optional: Add audit tracking just like your insert method
            _auditTracker?.AddEntry(
                tableName: "Card",
                recordId: cardId.ToString(),
                operationType: "UpdateStatus",
                userId: "2");

            return OperationResult.Ok();
        }

        public async Task<int?> GetCardIdByApplicationIdAsync(int applicationId)
        {
            SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

   

            int? cardId = await connection.ExecuteScalarAsync<int?>
                (Query.Card.GetCardIdByApplicationId, new { ApplicationID = applicationId },CurrentTransaction);

            return cardId;
        }
    }
}
