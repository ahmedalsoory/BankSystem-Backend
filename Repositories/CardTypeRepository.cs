using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.CardType;
using DTOs.OnboardingTypeSteps;
using Microsoft.Data.SqlClient;
using Repositories.Queries;
using Dapper;
using RepositoryContracts.CardType;

namespace Repositories
{
    public class CardTypeRepository: BaseRepository, ICardTypeReadRepository
    {
        public CardTypeRepository(IDbContextScope dbContextScope, Context error
          , IOptions<DbSettings> options) : base(dbContextScope, error,connectionOptions: options)
        {

        }

        public async Task<IEnumerable<CardTypeResponse>> GetAll()
        {
            base.SetAction();
            using var connection = new SqlConnection(base._connectionString);
            await connection.OpenAsync();
           
            return await connection.QueryAsync<CardTypeResponse>(
                Query.CardType.GetAll);
        }

    }
}
