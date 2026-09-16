using DTOs.interfaces;
using DTOs.Transaction.interfaces;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Repositories.Queries;
using Repositories.Queries.QueriesValiditon;

namespace Repositories.Validations
{
    public class TransactionValidationRepository : BaseRepository,
        IValidationService<ITransactionValidationDTO>
    {
        public TransactionValidationRepository(IDbConnectionProvider provider, Context error)
          : base(provider, error) { }

        public async Task<List<string>> ValidateAsync(ITransactionValidationDTO dto, int? id = null)
        {
            var errors = new List<string>();

            // 1. Check if the account has sufficient balance
            if (!await HasSufficientBalanceAsync(dto))
            {
                errors.Add("Validation Failed: Insufficient funds for this transaction.");
                return errors;
            }

            return errors;
        }

        private async Task<bool> HasSufficientBalanceAsync(ITransactionValidationDTO dto)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();

            // Using QueryFirstOrDefaultAsync to check the balance query we built earlier
            var result = await connection.QueryFirstOrDefaultAsync<int?>(
                   QueryValidtion.TransactionValidation.IsBalanceHasWithdrawAmount,
                new { AccountId = dto.FromAccountId, Amount = dto.Amount },
                base.CurrentTransaction
            );

            return result.HasValue;
        }
    }
}
