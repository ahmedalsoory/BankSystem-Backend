using Azure.Core;
using Dapper;
using DTOs.AccountApplications.interfaces;
using DTOs.AccountOpeningDetail;
using DTOs.AccountOpeningDetail.interfaces;
using DTOs.interfaces;
using Repositories.Queries.QueriesValiditon;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Validations
{
    public class AccountOpeningDetailValidtion : BaseRepository, IValidationService<IAccountOpeningDetailValidtionDTO>
    {
        public AccountOpeningDetailValidtion(IDbConnectionProvider provider, Context error)
            : base(provider, error) { }

        public async Task<List<string>> ValidateAsync(IAccountOpeningDetailValidtionDTO dto, int? id = null)
        {
            var errors = new List<string>();

            // HUP Handshake: Validate business state before entering any Unit of Work
            var duplicateError = await CheckForActiveDetailAsync(dto);
            if (!string.IsNullOrEmpty(duplicateError))
            {
                errors.Add(duplicateError);
            }

            return errors;
        }

        private async Task<string?> CheckForActiveDetailAsync(IAccountOpeningDetailValidtionDTO request)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();

            // HUP Execution: Use externalized query string with explicit transaction: null
            int count = await connection.ExecuteScalarAsync<int>(
                QueryValidtion.AccountOpeningDetailValidation.CheckActiveDetail,
                request,
                transaction: null // Guarantees this read is independent of the main transaction pipe
            );

            return count > 0
                ? $"A record for '{request.RequirementKey}' already exists and is not rejected."
                : null;
        }
    }
}
