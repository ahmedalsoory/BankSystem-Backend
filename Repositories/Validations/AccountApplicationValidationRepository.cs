using Dapper;
using DTOs.AccountApplications.interfaces;
using DTOs.interfaces;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Repositories.Queries;
using Repositories.Queries.QueriesValiditon;

namespace Repositories.Validations
{
    public class AccountApplicationValidationRepository :
        BaseRepository, IValidationService<IAccountApplicationValidationDTO>
    {
        public AccountApplicationValidationRepository(
            IDbConnectionProvider provider,
            Context error) : base(provider, error) { }

        private async Task<bool> IsApplicationAlreadyPendingAsync(int accountId, byte applicationTypeId)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
        
        
            // 🚀 CRITICAL FIX 3: Always forward the transaction instance object down to Dapper 
            // to ensure it enlists inside the current middleware transaction context.
            int exists = await connection.ExecuteScalarAsync<int>(
             QueryValidtion.AccountApplicationValidation.CheckPendingApplication,
             new { AccountID = accountId, ApplicationTypeID = applicationTypeId }
         );
            return exists == 1;
        }

        public async Task<List<string>> ValidateAsync(IAccountApplicationValidationDTO dto, int? id = null)
        {
            var errors = new List<string>(1);

            // Domain Rule: Prevent submitting duplicate applications of the same type while one is already pending processing
            bool isPending = await IsApplicationAlreadyPendingAsync(dto.AccountID, dto.ApplicationTypeID);
            if (isPending)
            {
                errors.Add($"Validation Failed: A pending application for this specific account type is already active.");
            }

            return errors;
        }
    }
}