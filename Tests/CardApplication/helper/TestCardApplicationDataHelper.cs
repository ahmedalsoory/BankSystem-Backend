using Dapper;
using Microsoft.Data.SqlClient;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Account.helper;

namespace Tests.CardApplication.helper
{
   
    public static class TestCardApplicationDataHelper
    {
        public static async Task<int> getLastAccountId(IDbConnectionProvider provider)
        {

            var connection = await provider.GetConnectionAsync();
            await provider.BeginTransactionAsync();
            const string sql = "select Top 1 AccountID from Banking.Accounts order by AccountID desc";
            return await connection.QueryFirstOrDefaultAsync<int>(sql,transaction:provider.CurrentTransaction);


        }
    }
}
