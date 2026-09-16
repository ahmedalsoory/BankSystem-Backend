using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DTOs.OnboardingTypeSteps;
using DTOs.ApplicationTypes;
using Microsoft.Data.SqlClient;
using Repositories.Queries;
using Dapper;
using RepositoryContracts.OnboardingTypeSteps;

namespace Repositories
{
    public class OnboardingTypeStepsRepository : BaseRepository , IOnboardingTypeStepsReadRepository
    {
        public OnboardingTypeStepsRepository(IDbConnectionProvider dbConnection, Context error,IOptions
            <DbSettings> options): base(dbConnection,error,connectionOptions: options) 
        {
            
        }

        public async Task<IEnumerable<OnboardingTypeStepsResponse>> GetAll()
        {
            base.SetAction();
            using var connection = new SqlConnection(base._connectionString);
            await connection.OpenAsync();

            return await connection.QueryAsync<OnboardingTypeStepsResponse>(
                Query.OnboardingTypeSteps.GetAll);
        }

    }
}
