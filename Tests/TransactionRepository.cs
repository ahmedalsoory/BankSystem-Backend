using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryContracts.Transaction;
using Dapper;
using DTOs.Transaction;
using Shared.Enums.Account;
using DTOs;
using Microsoft.Data.SqlClient;
using Shared.Enums.Transaction;
using Shared.Enums;
using System.Data;
using Repositories.Queries;

namespace Repositories
{
    public class TransactionRepository : BaseRepository, ITransactionWriteRepository
        , ITransactionReadRepository
    {
        public TransactionRepository(IDbContextScope dbContextScope, Context error
            , IOptions<DbSettings> options) : base(dbContextScope, error, options) { }

        public async Task<OperationResult> DepositAsync(DepositRequest request, int executedByUserId = 1)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            int affected = await connection.ExecuteAsync(Query.Transaction.UpdateBalance,
                new { Id = request.ToAccountId, Amount = request.Amount }, base.CurrentTransaction);

            if (affected == 0) return OperationResult.Failure("Deposit failed: Account not found.");

            await connection.ExecuteAsync(Query.Transaction.LogTransaction, new
            {
                request.Amount,
                FromAccountId = (int?)null, // Explicitly pass NULL
                request.ToAccountId,
                UserId = executedByUserId,
                request.SourceId,
                Type =enTransactionType.Deposit  ,
                request.sourceType,
            }, base.CurrentTransaction);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> WithdrawAsync(WithdrawRequest request, int executedByUserId)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            int affected = await connection.ExecuteAsync(Query.Transaction.WithdrawBalance,
                new { Id = request.FromAccountId, Amount = request.Amount }, base.CurrentTransaction);

            if (affected == 0) return OperationResult.Failure("Withdrawal failed: Insufficient funds or account not found.");

            await connection.ExecuteAsync(Query.Transaction.LogTransaction, new
            {
                request.Amount,
                Type = (int)enTransactionType.Withdraw,
                FromAccountId = request.FromAccountId,
                ToAccountId = (int?)null,
                request.SourceId,
                request.sourceType,
                UserId = executedByUserId
            }, base.CurrentTransaction);

            return OperationResult.Ok();
        }

        public async Task<OperationResult> TransferAsync(TransferRequest request, int executedByUserId)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            // Assuming Transaction handling is orchestrated by the Service Layer/Scope

            // 1. Withdraw from Sender
            int withdraw = await connection.ExecuteAsync(Query.Transaction.WithdrawBalance,
                new { Id = request.FromAccountId, Amount = request.Amount }, base.CurrentTransaction);
            if (withdraw == 0) return OperationResult.Failure("Transfer failed: Insufficient funds.");

            // 2. Deposit to Receiver
            int deposit = await connection.ExecuteAsync(Query.Transaction.UpdateBalance,
                new { Id = request.ToAccountId, Amount = request.Amount }, base.CurrentTransaction);
            if (deposit == 0) throw new Exception("Receiver account not found."); // Triggers rollback

            // 3. Log
            await connection.ExecuteAsync(Query.Transaction.LogTransaction, new
            {
                request.Amount,
                Type = (int)enTransactionType.Transfer,
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                UserId = executedByUserId,
                SourceId = request.SourceId,
                SourceType = request.sourceType
            }, base.CurrentTransaction);

            return OperationResult.Ok();
        }
        public async Task<PagedResult<ClientTransactionResponse>> GetTransactionsPagedAsync(
        TransactionScope scope, int? targetId, int pageNumber, int pageSize,
        SortedBy_Transaction sortBy, Direction direction,
        Filter_Transaction? filterBy = null, string? filterValue = null,
        string? fromDate = null, string? toDate = null)
        {
            base.SetAction();



            string sortDir = direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = sortBy switch
            {
                SortedBy_Transaction.Amount => $"Amount {sortDir}",
                SortedBy_Transaction.Type => $"[Type] {sortDir}",
                _ => $"TransactionDate {sortDir}"
            };



            var p = new DynamicParameters();
            var whereClauses = new List<string>();

            // 1. Scope logic
        

           

            // 2. Dynamic Filter Logic
            if (filterBy.HasValue && !string.IsNullOrEmpty(filterValue))
            {
                switch (filterBy)
                {
                    case Filter_Transaction.Type:
                        // Inject the actual integer value, not the placeholder @Type
                        whereClauses.Add($"t.[Type] = {int.Parse(filterValue)}");
                        break;
                    case Filter_Transaction.MinAmount:
                        whereClauses.Add($"t.Amount >= {decimal.Parse(filterValue)}");
                        break;
                    case Filter_Transaction.MaxAmount:
                        whereClauses.Add($"t.Amount <= {decimal.Parse(filterValue)}");
                        break;
                    case Filter_Transaction.AccountId:
                        int accId = int.Parse(filterValue);
                        whereClauses.Add($"(t.FromAccountId = {accId} OR t.ToAccountId = {accId})");
                        break;
                    
                     
                }
            }


           if(filterBy== Filter_Transaction.TransactionDate)
            {

                if (DateTime.TryParse(fromDate, out var start) && DateTime.TryParse(toDate, out var end))
                {
                    // Add one day to end to make it inclusive (e.g., up to end of the day)
                    string endFormatted = end.AddDays(1).ToString("yyyy-MM-dd");
                    string startFormatted = start.ToString("yyyy-MM-dd");

                    whereClauses.Add($"t.TransactionDate >= '{startFormatted}' AND t.TransactionDate < '{endFormatted}'");
                }
            }


                string whereSql = whereClauses.Any() ? " AND " + string.Join(" AND ", whereClauses) : "";

            p.Add("@WhereClause", whereSql);
            p.Add("@TargetId", targetId);
            p.Add("@Offset", (pageNumber - 1) * pageSize);
            p.Add("@PageSize", pageSize);
            p.Add("@OrderBy", orderByClause);
            p.Add("@Scope", (int)scope);
            using (var connection = new SqlConnection(base._connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "dbo.sp_GetClientTransactionsPaged", p, commandType: CommandType.StoredProcedure))
                {
                    int totalCount = await multi.ReadFirstAsync<int>();
                    var data = (await multi.ReadAsync<ClientTransactionResponse>()).ToList();

                    return new PagedResult<ClientTransactionResponse> { Data = data, TotalCount = totalCount };
                }
            }
        }

        // Updated Helper: returns null instead of DBNull.Value
        private object? GetFilterValue<T>(Filter_Transaction? filterBy, string? value, Filter_Transaction target)
        {
            if (filterBy == target && !string.IsNullOrWhiteSpace(value))
            {
                if (typeof(T) == typeof(int?) && int.TryParse(value, out int i)) return i;
                if (typeof(T) == typeof(decimal?) && decimal.TryParse(value, out decimal d)) return d;
            }
            return null; // Dapper handles nulls as SQL NULLs automatically
        }



        public IAsyncEnumerable<ClientTransactionResponse> StreamTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /*
        public async Task<PagedResult<ClientTransactionResponse>> GetTransactionsPagedAsync(
         TransactionScope scope,
         int? targetId,
         int pageNumber,
         int pageSize,
         SortedBy_Transaction sortBy,
         Direction direction,
         Filter_Transaction? filterBy = null,
         string? filterValue = null,
         string? fromDate = null,
         string? toDate = null)
        {
            base.SetAction();

            List<ClientTransactionResponse> transactions = null!;
            int totalCount = 0;

            await ExecuteTransactionReaderAsync(
                scope,
                targetId,
                pageNumber,
                pageSize,
                sortBy,
                direction,
                filterBy,
                filterValue,
                fromDate,
                toDate,

                (expectedAllocationSize) =>
                {
                    transactions = new List<ClientTransactionResponse>(expectedAllocationSize);
                },

                async (reader) =>
                {
                    transactions.Add(new ClientTransactionResponse
                    {
                        Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                        Type = (byte)reader["Type"],
                        TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                        FromAccountId = reader.IsDBNull(reader.GetOrdinal("FromAccountId")) ? null : reader.GetInt32(reader.GetOrdinal("FromAccountId")),
                        ToAccountId = reader.IsDBNull(reader.GetOrdinal("ToAccountId")) ? null : reader.GetInt32(reader.GetOrdinal("ToAccountId"))
                    });
                },

                (total) => totalCount = total
            );

            return new PagedResult<ClientTransactionResponse>
            {
                Data = transactions ?? new List<ClientTransactionResponse>(0),
                TotalCount = totalCount
            };
        }

        private async Task ExecuteTransactionReaderAsync(
      TransactionScope scope,
      int? targetId,
      int pageNumber,
      int pageSize,
      SortedBy_Transaction sortBy,
      Direction direction,
      Filter_Transaction? filterBy,
      string? filterValue,
      string? fromDate,
      string? toDate,
      Action<int> onCountCalculated,
      Func<SqlDataReader, Task> rowProcessor,
      Action<int> totalCountCallback)
        {
            string? connectionString = base._connectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                var tempConnection = await base.GetConnectionAsync();
                connectionString = tempConnection.ConnectionString;
            }

            string sortDir = direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = sortBy switch
            {
                SortedBy_Transaction.Amount => $"Amount {sortDir}",
                SortedBy_Transaction.Type => $"[Type] {sortDir}",
                SortedBy_Transaction.TransactionDate => $"TransactionDate {sortDir}",
                _ => $"TransactionDate {sortDir}"
            };

            object sqlTypeParam = DBNull.Value;
            object sqlMinAmountParam = DBNull.Value;
            object sqlMaxAmountParam = DBNull.Value;
            object sqlFromDateParam = DBNull.Value;
            object sqlToDateParam = DBNull.Value;

            if (!string.IsNullOrWhiteSpace(fromDate) && DateTime.TryParse(fromDate, out DateTime startParsed))
                sqlFromDateParam = startParsed.Date;

            if (!string.IsNullOrWhiteSpace(toDate) && DateTime.TryParse(toDate, out DateTime endParsed))
                sqlToDateParam = endParsed.Date.AddDays(1);

            if (filterBy != null && !string.IsNullOrWhiteSpace(filterValue))
            {
                string trimmedValue = filterValue.Trim();

                switch (filterBy.Value)
                {
                    case Filter_Transaction.Type:
                        if (byte.TryParse(trimmedValue, out byte typeByte)) sqlTypeParam = (int)typeByte;
                        break;

                    case Filter_Transaction.MinAmount:
                        if (decimal.TryParse(trimmedValue, out decimal minAmt)) sqlMinAmountParam = minAmt;
                        break;

                    case Filter_Transaction.MaxAmount:
                        if (decimal.TryParse(trimmedValue, out decimal maxAmt)) sqlMaxAmountParam = maxAmt;
                        break;

                    case Filter_Transaction.AccountId:
                        if (int.TryParse(trimmedValue, out int parsedAccId))
                        {
                            scope = TransactionScope.AccountDetails;
                            targetId = parsedAccId;
                        }
                        break;
                }
            }

            using (var connection = new SqlConnection(connectionString))
            {
                // 🚀 CALLING: Stored procedure targeting your paged transactions architecture
                using (var command = new SqlCommand("dbo.sp_GetClientTransactionsPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Scope", (int)scope);
                    command.Parameters.AddWithValue("@TargetId", targetId.HasValue ? targetId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@OrderBy", orderByClause);

                    command.Parameters.AddWithValue("@TypeFilter", sqlTypeParam);
                    command.Parameters.AddWithValue("@MinAmountFilter", sqlMinAmountParam);
                    command.Parameters.AddWithValue("@MaxAmountFilter", sqlMaxAmountParam);
                    command.Parameters.AddWithValue("@FromDateFilter", sqlFromDateParam);
                    command.Parameters.AddWithValue("@ToDateFilter", sqlToDateParam);

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
        */
    }
}