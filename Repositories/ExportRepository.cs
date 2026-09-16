using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using RepositoryContracts.ExportRepository;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ExportRepository :BaseRepository ,IExportRepository
    {


        public ExportRepository(IDbContextScope dbContextScope, Context error
          , IOptions<DbSettings> options) : base(dbContextScope, error, connectionOptions: options) { }



        public async IAsyncEnumerable<T> StreamQueryAsync<T>(
            string sqlQuery,
            object? parameters = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            SetAction();
          

            

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
           
            // Dapper's QueryUnbufferedAsync streams rows on-demand with an active connection open
            var asyncEnumerable = connection.QueryUnbufferedAsync<T>(
                sql: sqlQuery,
                param: parameters,
                commandTimeout: 300 // 5-minute timeout for massive datasets
            );

            await foreach (var item in asyncEnumerable.WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}