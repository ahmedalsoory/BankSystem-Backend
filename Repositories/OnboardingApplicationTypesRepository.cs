using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.ApplicationTypes;
using Microsoft.Data.SqlClient;
using Repositories.Queries;
using Dapper;
using DTOs.OnboardingApplicationTypes;
using RepositoryContracts.OnboardingApplicationTypes;

namespace Repositories
{
    public class OnboardingApplicationTypesRepository  :BaseRepository, IOnboardingApplicationTypesReadRepository
    {
        public OnboardingApplicationTypesRepository(IDbConnectionProvider dbContextScope,
            Context error,
            IOptions<DbSettings> options) : base(dbContextScope, error, connectionOptions: options) { }


        public async Task<IEnumerable<OnboardingApplicationTypesListItem>> GetAll()
        {
            base.SetAction();
            using var connection = new SqlConnection(base._connectionString);
            await connection.OpenAsync();

            return await connection.QueryAsync<OnboardingApplicationTypesListItem>(
                Query.OnboardingApplicationTypes.GetAll);
        }


    }
}
