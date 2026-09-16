using DTOs.CardApplication.interfaces;
using DTOs.interfaces;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Repositories.Queries.QueriesValiditon;
using System.Numerics;
using Repositories.Queries;

namespace Repositories.Validations
{
    public class CardApplicationValidationRepository : BaseRepository,
      IValidationService<ICardApplicationValidation>
    {
        public CardApplicationValidationRepository(IDbConnectionProvider provider,
            Context error) : base(provider, error) { }

        public async Task<List<string>> ValidateAsync(ICardApplicationValidation dto, int? id = null)
        {
            var errors = new List<string>();

            // Run all validation methods and aggregate errors
            if (!dto.oldCardId.HasValue)
            {
                errors.AddRange(await IsBalanceSufficient(dto));

            }
            else
                errors.AddRange(await IsCardForAccount(dto));


            return errors;
        }

        private async Task<List<string>> IsCardForAccount(ICardApplicationValidation entity)
        {
            var errors = new List<string>();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            bool exists = await connection.ExecuteScalarAsync<bool>(
                QueryValidtion.CardApplicationValidtion.IsCardOwnedByAccount,
                new { OldCardId = entity.oldCardId, AccountId = entity.AccountID }
            ,base.CurrentTransaction);

            if (!exists)
            {
                errors.Add("The selected card does not belong to this account.");
            }

            return errors;
        }

        private async Task<List<string>> IsBalanceSufficient(ICardApplicationValidation entity)
        {
            var errors = new List<string>();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();
            bool isValid = await connection.ExecuteScalarAsync<bool>(
                QueryValidtion.CardApplicationValidtion.IsBalanceSufficient,
                new { CardTypeID = entity.CardTypeID, AccountId = entity.AccountID }
            ,base.CurrentTransaction);

            if (!isValid)
            {
                errors.Add("The account balance is insufficient for this card type.");
            }

            return errors;
        }
        private async Task<List<string>> HasActiveCardOfTypeAsync(ICardApplicationValidation entity)
        {
            var errors = new List<string>();
            var connection = await base.GetConnectionAsync();

            bool hasCard = await connection.ExecuteScalarAsync<bool>(
                QueryValidtion.CardApplicationValidtion.HasActiveCardOfType,
                new { entity.AccountID, entity.CardTypeID },
                base.CurrentTransaction
            );

            if (hasCard)
            {
                errors.Add("The account already has an active card of this type.");
            }

            return errors;
        }
    }
}
