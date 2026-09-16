using Dapper;
using DTOs.interfaces;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using DTOs.Account.interfaces;
using Repositories.Queries.QueriesValiditon;

namespace Repositories.Validations
{
    public class AccountValidationRepository : BaseRepository, IValidationService<IAccountValidtionDTO>
    {
        public AccountValidationRepository(IDbConnectionProvider provider,
            Context error) : base(provider, error) { }

        public async Task<int> NumberOfAccountForClient(int clientID)
        {
            var connection = await base.GetConnectionAsync();
             await base.BeginTransactionAsync();

            return await connection.ExecuteScalarAsync<int>(
                QueryValidtion.AccountValidation.CountAccountsForClient,
                new { ClientID = clientID },base.CurrentTransaction
            );
        }

        public async Task<List<string>> ValidateAsync(IAccountValidtionDTO dto, int? id = null)
        {
            var errors = new List<string>();

            int accountCount = await NumberOfAccountForClient(dto.ClientID);

            if (accountCount >= 5)
            {
                errors.Add("Validation Failed: The client has already reached the maximum allowed number of accounts (Limit: 5).");
            }

            return errors;
        }
    }
}