using RepositoryContracts.AccountRepo;
using DTOs.Account;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using Azure.Core;
using Shared.Enums.Account;
using DTOs.Transaction;
using RepositoryContracts.Transaction;
using DTOs;
using Shared.Enums;
using Shared.Enums.Transaction;
using DTOs.interfaces;
using DTOs.Account.interfaces;
using Shared.Exceptions;
using Repositories.Queries;

namespace Repositories.Banking
{
    internal sealed class AccountRepository : BaseRepository, IAccountReadRepository,
        IAccountWriteRepository, IAccountLockRepository
    {
        private readonly IValidationService<IAccountValidtionDTO> _validationService;
        public AccountRepository(IDbContextScope dbContextScope, Context error
            ,IAuditTracker auditTracker, IValidationService<IAccountValidtionDTO> 
            validationService,IOptions<DbSettings> options) : base(dbContextScope, error, auditTracker,options)
        {
            _validationService = validationService;
        }

        public async Task<AccountResponse?> GetByIdAsync(int id)
        {
            SetAction();
            return await ExecuteTransientAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<AccountResponse>(
                    Query.Account.GetById,
                    new { id }
                );
            });
            //  return await base.ManualExecuteAsync<AccountResponse>(Query.Account.GetById, new { id });
        }

        public async Task LockAccountForTransferAsync(int fromAccountId, int toAccountId)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();
            await connection.ExecuteAsync(
                    Query.Account.LockAccountForTransfer,
                    new { FromAccountId = fromAccountId, ToAccountId = toAccountId },
                    CurrentTransaction
                );
        }
        public async Task<AccountResponse?> GetByNumberAsync(string accountNumber)
        {
            SetAction();
            //throw new Exception();
            return await ExecuteTransientAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<AccountResponse>(
                    Query.Account.GetByNumber,
                    new { accountNumber }
                );
            });
        }

        public async Task<IEnumerable<AccountListItem>> GetByClientIdAsync(int clientId)
        {
            SetAction();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryAsync<AccountListItem>(
                Query.Account.GetByClientId, new { clientId });
        }

        public async Task<OperationResult<int>> AddAsync(AccountAddRequest account)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int newAccountId = await connection.QuerySingleAsync<int>(
                Query.Account.Add,
                account,
                transaction: CurrentTransaction);

            if (newAccountId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
             tableName: "Account",
             recordId: newAccountId.ToString(),
             operationType: "Insert",
             userId: "2");


            return OperationResult<int>.Ok(newAccountId);
        }

        public async Task<OperationResult> UpdateStatus(AccountUpdateStatusRequest request)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();
            var affectedRows = await connection.ExecuteAsync(
                          Query.Account.UpdateStatus, request, CurrentTransaction);

            if (affectedRows == 0) return OperationResult.Failure("account update status faild");
            _auditTracker?.AddEntry(
             tableName: "Account",
             recordId: request.AccountID.ToString(),
             operationType: "Update",
             userId: "2");


            return OperationResult.Ok();
        }

        public async Task<string> GetNextAccountNumberAsync()
        {
            var connection = await GetConnectionAsync();
            var seqParams = new DynamicParameters();
            seqParams.Add("@SchemaName", "Banking");
            seqParams.Add("@TableName", "Accounts");
            seqParams.Add("@ColumnName", "AccountNumber");
            seqParams.Add("@Prefix", "ACT-");
            seqParams.Add("@Length", 8);
            seqParams.Add("@Result", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

            await connection.ExecuteAsync(
                sql: "[dbo].[GetNextSequenceValue]",
                param: seqParams,
                transaction: CurrentTransaction, // Ensures it's part of the current transaction
                commandType: CommandType.StoredProcedure
            );

            return seqParams.Get<string>("@Result");
        }

        public async Task<PagedResult<AccountListItem>> GetAccountsPagedAsync(
              AccountPagedRequest request)
        {
            SetAction(nameof(GetAccountsPagedAsync));

            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                SortedBy_Account.AccountNumber => $"A.AccountNumber {sortDir}",
                SortedBy_Account.Balance => $"A.Balance {sortDir}",
                _ => $"A.CreatedDate {sortDir}"
            };

            // 1. Build the Where Clause and capture the parameter value
            var (whereClause, paramValue) = BuildAccountWhereClause(request.FilterBy, request.FilterValue);

            // 2. Use DynamicParameters with explicit DbTypes
            var parameters = new DynamicParameters();
            parameters.Add("@Offset", (request.PageNumber - 1) * request.PageSize, DbType.Int32);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32);
            parameters.Add("@OrderBy", orderByClause, DbType.String);
            parameters.Add("@WhereClause", whereClause, DbType.String);

            // Explicitly handle @Value: If paramValue is null, Dapper needs the type
            var dbType = paramValue is int ? DbType.Int32 :
                         paramValue is decimal ? DbType.Decimal : DbType.String;

            parameters.Add("@Value", paramValue, dbType, ParameterDirection.Input);

            // 🎯 Pass the cached total count down to the stored procedure
            parameters.Add("@CachedTotalCount", request.CachedTotalCount, DbType.Int32, ParameterDirection.Input);

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "dbo.sp_GetAccountsPaged",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    int totalCount = await multi.ReadFirstAsync<int>();
                    var data = (await multi.ReadAsync<AccountListItem>()).ToList();

                    return new PagedResult<AccountListItem> { Data = data, TotalCount = totalCount };
                }
            }
        }
        private (string WhereClause, object? ParamValue) BuildAccountWhereClause(Filter_Account? filterBy, string? filterValue)
        {
            if (filterBy == null || string.IsNullOrWhiteSpace(filterValue))
                return ("WHERE 1=1", null); // Return null, NOT DBNull.Value

            string trimmed = filterValue.Trim();
            return filterBy.Value switch
            {
                Filter_Account.AccountNumber => ("WHERE A.AccountNumber LIKE @Value + '%'", trimmed),
                Filter_Account.ClientID => int.TryParse(trimmed, out int cid) ? ("WHERE A.ClientID = @Value", cid) : ("WHERE 1=1", null),
                Filter_Account.AccountType => int.TryParse(trimmed, out int tid) ? ("WHERE A.AccountType = @Value", tid) : ("WHERE A.AccountType LIKE @Value + '%'", trimmed),
                Filter_Account.Status => int.TryParse(trimmed, out int sid) ? ("WHERE A.Status = @Value", sid) : ("WHERE 1=1", null),
                _ => ("WHERE 1=1", null)
            };
        }
        /*
        public async Task<PagedResult<AccountListItem>> GetAccountsPagedAsync(
              int pageNumber,
              int pageSize,
              SortedBy_Account sortBy,
              Direction direction,
              Filter_Account? filterBy,
              string? filterValue)
        {
            List<AccountListItem> records = null!;
            int totalCount = 0;

            await ExecuteAccountReaderAsync(
                pageNumber,
                pageSize,
                sortBy,
                direction,
                filterBy,
                filterValue,

                (expectedAllocationSize) =>
                {
                    records = new List<AccountListItem>(expectedAllocationSize);
                },

                async (reader) =>
                {
                    records.Add(MapSqlReaderToAccountResponse(reader));
                },

                (count) =>
                {
                    totalCount = count;
                });

            return new PagedResult<AccountListItem>
            {
                Data = records ?? new List<AccountListItem>(0),
                TotalCount = totalCount
            };
        }

        private async Task ExecuteAccountReaderAsync(
            int pageNumber,
            int pageSize,
            SortedBy_Account sortBy,
            Direction direction,
            Filter_Account? filterBy,
            string? filterValue,
            Action<int> onCountCalculated,
            Func<SqlDataReader, Task> rowProcessor,
            Action<int> totalCountCallback)
        {
            // =============================================
            // 1. STRICT WHITE-LIST SORTING MAPS
            // =============================================
            string sortDir = direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = sortBy switch
            {
                SortedBy_Account.AccountNumber => $"A.AccountNumber {sortDir}",
                SortedBy_Account.Balance => $"A.Balance {sortDir}",
                SortedBy_Account.CreatedDate => $"A.CreatedDate {sortDir}",
                _ => $"A.CreatedDate {sortDir}"
            };

            // =============================================
            // 2. DYNAMIC WHERE CLAUSE COMPOSITION (C# Style)
            // =============================================
            string whereClause = "";
            object sqlValueParam = DBNull.Value;

            if (filterBy != null && !string.IsNullOrWhiteSpace(filterValue))
            {
                string trimmedValue = filterValue.Trim();

                switch (filterBy.Value)
                {
                    case Filter_Account.AccountNumber:
                        whereClause = "WHERE A.AccountNumber LIKE @Value + '%'";
                        sqlValueParam = trimmedValue;
                        break;

                    case Filter_Account.ClientID:
                        if (int.TryParse(trimmedValue, out int clientId))
                        {
                            whereClause = "WHERE A.ClientID = @Value";
                            sqlValueParam = clientId;
                        }
                        break;

                    case Filter_Account.AccountType:
                        if (int.TryParse(trimmedValue, out int typeId))
                        {
                            whereClause = "WHERE A.AccountType = @Value";
                            sqlValueParam = typeId;
                        }
                        else
                        {
                            whereClause = "WHERE A.AccountType LIKE @Value + '%'";
                            sqlValueParam = trimmedValue;
                        }
                        break;

                    case Filter_Account.Status:
                        if (int.TryParse(trimmedValue, out int statusId))
                        {
                            whereClause = "WHERE A.Status = @Value";
                            sqlValueParam = statusId;
                        }
                        break;
                }
            }

            // =============================================
            // 3. SECURE ADO.NET MULTI-RESULT EXECUTION BLOCK
            // =============================================
            using (var connection = new SqlConnection(base._connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_GetAccountsPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@OrderBy", orderByClause);
                    command.Parameters.AddWithValue("@WhereClause", whereClause);
                    command.Parameters.AddWithValue("@Value", sqlValueParam);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int totalFilteredRows = reader.GetInt32(0);
                            totalCountCallback(totalFilteredRows);

                            int expectedRowsOnPage = Math.Min(pageSize, totalFilteredRows - ((pageNumber - 1) * pageSize));
                            expectedRowsOnPage = Math.Max(0, expectedRowsOnPage);

                            onCountCalculated(expectedRowsOnPage);
                        }

                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                await rowProcessor(reader);
                            }
                        }
                    }
                }
            }
        }

        // =========================================================================
        // DATA ROW MAPPER
        // =========================================================================
        private AccountListItem MapSqlReaderToAccountResponse(SqlDataReader reader)
        {
            return new AccountListItem
            {
                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                AccountNumber = reader.GetString(reader.GetOrdinal("AccountNumber")),
                Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                AccountType = reader.GetByte(reader.GetOrdinal("AccountType")),
                Status = reader.GetByte(reader.GetOrdinal("Status")),
            };
        }
        */




    }
}