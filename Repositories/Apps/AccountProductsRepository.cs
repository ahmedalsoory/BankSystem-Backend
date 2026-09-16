using Dapper;
using DTOs.AccountProducts;
using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Queries;
using RepositoryContracts.AccountProducts;

namespace Repositories.Apps
{
    public class AccountProductsRepository : BaseRepository, IAccountProductsReadTransactionalRepository
    {
        public AccountProductsRepository(
            IDbContextScope dbContextScope,
            Context error,
            IOptions<DbSettings> options) : base(dbContextScope, error,connectionOptions: options)
        {
        }

        public async Task<AccountProductsResponse?> GetByIDAsyncTransactional(int id)
        {
            SetAction();

            // Get the connection currently managed by the scope/transaction
            var connection = await GetConnectionAsync();

            // Perform a Dapper query using the existing transaction
            // This ensures that if you are in the middle of an "Account Opening" transaction,
            // you are reading the most up-to-date configuration data.
            return await connection.QueryFirstOrDefaultAsync<AccountProductsResponse>(
               Query.AccountProducts.GetByID,
                new { Id = id },
                transaction: CurrentTransaction);
        }
    }
}
