using Azure.Core;
using Dapper;
using DTOs;
using DTOs.Client;

using DTOs.interfaces;
using DTOs.Person;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repositories.Queries;
using RepositoryContracts.ClientRepo;
using RepositoryContracts.PersonRepo;
using Shared;
using Shared.Enums;
using Shared.Enums.Client;
using Shared.Exceptions;
using Shared.Interfaces;
using System;
using System.Data;
using System.Globalization;
using System.Threading.Channels;

namespace Repositories.Core
{
    public sealed class ClientRepository : BaseRepository,
        IClientReadRepository, IClientWriteRepository
    {


        public ClientRepository(IDbConnectionProvider dbContextScope, Context error
         , IAuditTracker auditTracker,IOptions<DbSettings> settings) : base(dbContextScope, error,
             auditTracker, settings)
        {

        }

        public async Task<ClientResponse?> GetByAccountNumberAsync(string accountNumber)
        {
            SetAction();
            using var connection = new SqlConnection(base._connectionString);
            await connection.OpenAsync(base.CancellationToken);

            var command = new CommandDefinition(
             Query.Client.GetByAccountNumber,
             new { accountNumber },
             cancellationToken: base.CancellationToken // Pass it here
            );

            return await connection.QueryFirstOrDefaultAsync<ClientResponse>(command);
        }

        public async Task<ClientDetailDto?> GetByPersonIDAsync(int Id)
        {
            SetAction();
            using var connection = new SqlConnection(base._connectionString);
            await connection.OpenAsync(base.CancellationToken);

            var command = new CommandDefinition(
        Query.Client.GetByPersonId,
        new { Id },
        cancellationToken: base.CancellationToken // 👈 Passes token to Dapper query
    );


            return await connection.QueryFirstOrDefaultAsync<ClientDetailDto>(command);
        }

        public async Task<string> GetNextClientNumberAsync()
        {
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            var seqParams = new DynamicParameters();
            seqParams.Add("@SchemaName", "Core");
            seqParams.Add("@TableName", "Clients");
            seqParams.Add("@ColumnName", "ClientNumber");
            seqParams.Add("@Prefix", "C0-");
            seqParams.Add("@Length", 10);
            seqParams.Add("@Result", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);


            var command = new CommandDefinition(
        "[dbo].[GetNextSequenceValue]",
        seqParams,
        transaction: CurrentTransaction,
        commandType: CommandType.StoredProcedure,
        cancellationToken: CancellationToken // Automatically pulled from the HTTP pipe!
    );


            await connection.ExecuteAsync(command);

            return seqParams.Get<string>("@Result");
        }

        public async Task<OperationResult> RegisterClientAsync(ClientAddRequest request)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();


            var command = new CommandDefinition(
        Query.Client.Register,
        new
        {
            request.PersonID,
            request.ClientNumber,
            request.RiskLevel,
            request.IsActive,
            CreatedByUserID = 1
        },
        transaction: CurrentTransaction,
        cancellationToken: CancellationToken // Automatically pulled from the HTTP pipe!
    );


            int rowEffect= await connection.ExecuteAsync(command);

            if (rowEffect == 0 ) return OperationResult<int>.Failure("add client operaion faild");
            _auditTracker?.AddEntry(
             tableName: "Client",
             recordId: request.PersonID.Value.ToString(),
             operationType: "Insert",
             userId: "2");



            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateClientAsync(ClientUpdateRequest request)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            var command = new CommandDefinition(
        Query.Client.Update,
        request,
        transaction: CurrentTransaction,
        cancellationToken: CancellationToken // Automatically pulled from the HTTP pipe!
    );

            int affectedRows = await connection.ExecuteAsync(command);

            if (affectedRows == 0) return OperationResult<int>.Failure("RowVersion mismatch or missing client");
            _auditTracker?.AddEntry(
             tableName: "Client",
             recordId: request.Id.ToString(),
             operationType: "Update",
             userId: "2");

            return OperationResult.Ok();
        }
     

        public async Task<PagedResult<ClientListItemDto>> GetClientsPagedAsyncAsList(
              ClientPagedRequest request)
        {
            SetAction(nameof(GetClientsPagedAsyncAsList));

            // 1. Build dynamic components
            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                SortedBy_Client.Name => $"P.FirstName {sortDir}, P.LastName {sortDir}",
                SortedBy_Client.JoinedDate => $"C.JoinedDate {sortDir}",
                _ => $"C.JoinedDate {sortDir}"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@Offset", (request.PageNumber - 1) * request.pageSize);
            parameters.Add("@PageSize", request.pageSize);
            parameters.Add("@OrderBy", orderByClause);

            // Build the dynamic WHERE clause and parameters
            var (where, fn, ln, val) = BuildClientWhereClause(request.FilterBy, request.FilterValue);
            parameters.Add("@WhereClause", where);
            parameters.Add("@FN", fn);
            parameters.Add("@LN", ln);
            parameters.Add("@Value", val);

            // 🎯 Pass the cached total count down to the stored procedure
            parameters.Add("@CachedTotalCount", request.CachedTotalCount);

            // 2. Execute via Dapper
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(CancellationToken);
                var command = new CommandDefinition(
        "dbo.sp_GetClientsPaged",
        parameters,
        commandType: CommandType.StoredProcedure,
        cancellationToken: CancellationToken // 👈 Pass token to Dapper query
    );
                using (var multi = await connection.QueryMultipleAsync(command))
                {
                    int totalCount = await multi.ReadFirstAsync<int>();
                    IEnumerable<ClientListItemDto> data = totalCount == 0
    ? Array.Empty<ClientListItemDto>()
    : (await multi.ReadAsync<ClientListItemDto>()).ToList();

                    return new PagedResult<ClientListItemDto>
                    {
                        Data = data,
                        TotalCount = totalCount
                    };
                }
            }
        }

        private (string Where, object? FN, object? LN, object? Val) BuildClientWhereClause(Filter_Client? filterBy, string? filterValue)
        {
            if (filterBy == null || string.IsNullOrWhiteSpace(filterValue))
                return ("WHERE 1=1", null, null, null); // Return null instead of DBNull.Value

            string trimmed = filterValue.Trim();
            return filterBy.Value switch
            {
                Filter_Client.Name => trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries) switch
                {
                    var parts when parts.Length >= 2 => ("WHERE P.FirstName LIKE @FN + '%' AND P.LastName LIKE @LN + '%'", parts[0], string.Join(" ", parts.Skip(1)), null),
                    var parts when parts.Length == 1 => ("WHERE P.FirstName LIKE @FN + '%'", parts[0], null, null),
                    _ => ("WHERE 1=1", null, null, null)
                },
                Filter_Client.Email => ("WHERE P.Email LIKE @Value + '%'", null, null, trimmed),
                Filter_Client.ClientNumber => ("WHERE C.ClientNumber LIKE @Value + '%'", null, null, trimmed),
                Filter_Client.IsActive => ("WHERE C.IsActive = @Value", null, null, bool.TryParse(trimmed, out bool b) ? b ? 1 : 0 : int.Parse(trimmed)),
                _ => ("WHERE 1=1", null, null, null)
            };
        }
        /*
        public async Task<PagedResult<ClientListItemDto>> GetClientsPagedAsyncAsList(
             int pageNumber,
             int pageSize,
             SortedBy_Client sortBy,
             Direction direction,
             Filter_Client? filterBy = null,
             string? filterValue = null)
        {
            base.SetAction(nameof(GetClientsPagedAsyncAsList));

            List<ClientListItemDto> clients = null!;
            int totalCount = 0;

            await ExecuteReaderAsync(
                pageNumber,
                pageSize,
                sortBy,
                direction,
                filterBy,
                filterValue,

                (expectedAllocationSize) =>
                {
                    clients = new List<ClientListItemDto>(expectedAllocationSize);
                },

                async (reader) =>
                {
                    clients.Add(MapSqlReaderToClientResponse(reader));
                },

                (total) =>
                {
                    totalCount = total;
                });

            return new PagedResult<ClientListItemDto>
            {
                Data = clients ?? new List<ClientListItemDto>(0),
                TotalCount = totalCount
            };
        }

        private async Task ExecuteReaderAsync(
            int pageNumber,
            int pageSize,
            SortedBy_Client sortBy,
            Direction direction,
            Filter_Client? filterBy,
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
                SortedBy_Client.Name => $"P.FirstName {sortDir}, P.LastName {sortDir}",
                SortedBy_Client.JoinedDate => $"C.JoinedDate {sortDir}",
                _ => $"C.JoinedDate {sortDir}"
            };

            // =============================================
            // 2. DYNAMIC WHERE CLAUSE COMPOSITION (C# Style)
            // =============================================
            string whereClause = "";
            object sqlValueParam = DBNull.Value;
            object sqlFNParam = DBNull.Value;
            object sqlLNParam = DBNull.Value;

            if (filterBy != null && !string.IsNullOrWhiteSpace(filterValue))
            {
                string trimmedValue = filterValue.Trim();

                switch (filterBy.Value)
                {
                    case Filter_Client.Name:
                        var nameParts = trimmedValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (nameParts.Length >= 2)
                        {
                            whereClause = "WHERE P.FirstName LIKE @FN + '%' AND P.LastName LIKE @LN + '%'";
                            sqlFNParam = nameParts[0];
                            sqlLNParam = string.Join(" ", nameParts.Skip(1));
                        }
                        else if (nameParts.Length == 1)
                        {
                            whereClause = "WHERE P.FirstName LIKE @FN + '%'";
                            sqlFNParam = nameParts[0];
                        }
                        break;

                    case Filter_Client.Email:
                        whereClause = "WHERE P.Email LIKE @Value + '%'";
                        sqlValueParam = trimmedValue;
                        break;

                    case Filter_Client.ClientNumber:
                        whereClause = "WHERE C.ClientNumber LIKE @Value + '%'";
                        sqlValueParam = trimmedValue;
                        break;

                    case Filter_Client.IsActive:
                        if (bool.TryParse(trimmedValue, out bool isActiveBool))
                        {
                            whereClause = "WHERE C.IsActive = @Value";
                            sqlValueParam = isActiveBool ? 1 : 0;
                        }
                        else if (trimmedValue == "1" || trimmedValue == "0")
                        {
                            whereClause = "WHERE C.IsActive = @Value";
                            sqlValueParam = int.Parse(trimmedValue);
                        }
                        break;
                }
            }

            // =============================================
            // 3. SECURE ADO.NET EXECUTION BLOCK (MULTI-RESULT SET)
            // =============================================
            using (var connection = new SqlConnection(base._connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_GetClientsPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@OrderBy", orderByClause);
                    command.Parameters.AddWithValue("@WhereClause", whereClause);
                    command.Parameters.AddWithValue("@Value", sqlValueParam);
                    command.Parameters.AddWithValue("@FN", sqlFNParam);
                    command.Parameters.AddWithValue("@LN", sqlLNParam);

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

        private ClientListItemDto MapSqlReaderToClientResponse(SqlDataReader reader)
        {
            return new ClientListItemDto
            {
                PersonID = reader.GetInt32(reader.GetOrdinal("PersonId")),
                ClientNumber = reader.GetString(reader.GetOrdinal("ClientNumber")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                JoinDate = reader.GetDateTime(reader.GetOrdinal("JoinedDate"))
            };
        }
        */

    }
}