using Dapper;
using Shared.Interfaces;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs.interfaces;
using DTOs.AccountOpeningApplication.interfaces;
using Repositories.Queries.QueriesValiditon;

namespace Repositories.Validations
{
    public class AccountOpeningApplicationsValidationRepository : BaseRepository, IValidationService<IAccountOpeningValidationDTO>
    {
        public AccountOpeningApplicationsValidationRepository(IDbConnectionProvider provider, Context error)
            : base(provider, error) { }

        public async Task<List<string>> ValidateAsync(IAccountOpeningValidationDTO dto, int? id = null)
        {
            var errors = new List<string>();

            // 1. Handshake Phase: Perform non-transactional read validation
            if (!await ValidateNoActiveApplications(dto.ClientID, errors)) return errors;

            return errors;
        }

        private async Task<bool> ValidateNoActiveApplications(int clientID, List<string> errors)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();

            // Execute using externalized query
            int count = await connection.ExecuteScalarAsync<int>(
                QueryValidtion.AccountOpeningValidation.CheckActiveApplications, // HUP: SQL is external
                new { ClientID = clientID },
                transaction: null // HUP: Explicitly non-transactional for Handshake phase
            );

            if (count > 0)
            {
                errors.Add("Validation Failed: Client already has an active account opening application.");
                return false;
            }
            return true;
        }
    }
}