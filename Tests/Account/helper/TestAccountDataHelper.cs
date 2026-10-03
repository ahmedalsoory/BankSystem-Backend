using Dapper;
using DTOs.Account;
using System.Threading.Tasks;
using Shared.Interfaces; // Or your actual namespace for IDbConnectionProvider

namespace Tests.Account.helper
{
    public static class TestAccountDataHelper
    {
        public static async Task<AccountDtoForUpdate?> getAccountData(IDbConnectionProvider connectionProvider)
        {
            // Use the shared connection from the active transaction scope provider
            var connection = await connectionProvider.GetConnectionAsync(); // Or access active connection depending on your provider design

            await connectionProvider.BeginTransactionAsync();

            const string sql = "SELECT TOP 1 AccountID,Status, RowVersion FROM Banking.Accounts";
            return await connection.QueryFirstOrDefaultAsync<AccountDtoForUpdate>(sql,transaction:
                connectionProvider.CurrentTransaction);
        }

        public static async Task<string?> getAccountNumber(IDbConnectionProvider connectionProvider)
        {
             var connection = await connectionProvider.GetConnectionAsync();
             await connectionProvider.BeginTransactionAsync();

             const string sql = "SELECT TOP 1 AccountNumber FROM Banking.Accounts";
             return await connection.QueryFirstOrDefaultAsync<string>(sql, transaction:
                connectionProvider.CurrentTransaction);
        }
        public static async Task<int?> getAccountID(IDbConnectionProvider connectionProvider)
        {
             var connection = await connectionProvider.GetConnectionAsync();
             await connectionProvider.BeginTransactionAsync();

             const string sql = "SELECT TOP 1 AccountID FROM Banking.Accounts";
             return await connection.QueryFirstOrDefaultAsync<int>(sql, transaction:
                connectionProvider.CurrentTransaction);
        }
    }

    public class AccountDtoForUpdate
    {
        public int AccountID {  get; set; }
        public byte Status { get; set; }
        public byte[] RowVersion { get; set; }
    }
}