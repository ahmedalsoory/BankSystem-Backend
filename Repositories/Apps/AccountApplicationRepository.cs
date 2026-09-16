using Dapper;
using DTOs.AccountApplications;
using DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using RepositoryContracts.AccountApplications;
using Shared.Enums.AccountApplications;
using Shared.Enums;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using DTOs.AccountApplications.interfaces;
using DTOs.interfaces;
using Shared.Exceptions;
using Repositories.Queries;

namespace Repositories.Apps
{
    public class AccountApplicationRepository : BaseRepository,
        IAccountApplicationReadRepository,
        IAccountApplicationWriteRepository, 
        IAccountApplicationReadTransactionRepository
    {
        private readonly IValidationService<IAccountApplicationValidationDTO> _validation;
        
        public AccountApplicationRepository(
            IDbContextScope dbContextScope,
            Context error, IAuditTracker auditTracker,
            IOptions<DbSettings> options,
            IValidationService<IAccountApplicationValidationDTO> validation) : base(dbContextScope, error,
                auditTracker, options)
        {
            _validation = validation;
            
        }

        // =========================================================================
        // READ OPERATIONS (Dapper)
        // =========================================================================

        public async Task<AccountApplicationResponse?> GetByIdAsync(int id)
        {
            base.SetAction();
            // Uses the standardized HUP pattern
            return await ExecuteTransientAsync(async conn =>
              await conn.QueryFirstOrDefaultAsync<AccountApplicationResponse>(
                  Query.AccountApplication.GetById, new { id }));
        }
        public async Task<byte> GetApplicationTypeByIdAsync(int id)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            var applicationTypeId = await connection.QuerySingleAsync<byte>(
        Query.AccountApplication.GetApplicationTypeByIdAsync,
        new { ApplicationID = id },
        transaction: base.CurrentTransaction
    );
            
            return applicationTypeId;
        }

        // =========================================================================
        // WRITE OPERATIONS
        // =========================================================================

        public async Task<OperationResult<int>> AddAsync(AccountApplicationAddRequest application)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();


            int newId = await connection.ExecuteScalarAsync<int>(
                Query.AccountApplication.Add,
                new
                {
                    application.AccountID,
                    application.ApplicationTypeID,
                    application.CreatedByUserID,
                    application.Notes
                },
                transaction: CurrentTransaction);
            if (newId > 0)
            {

                _auditTracker.AddEntry(
            tableName: "AccountApplicationType",
            recordId: newId.ToString(),
            operationType: "INSERT",
            userId:  "2" // Falling back to "2" if null, or pass it from your request/context
        );
            }

            return OperationResult<int>.Ok(newId);
        }



        public async Task<bool> UpdateStatusAsync(AccountApplicationStatusUpdateRequest request)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();
           
                int affectedRows = await connection.ExecuteAsync(
                    Query.AccountApplication.UpdateStatus,
                    new
                    {
                        Id = request.ApplicationId,
                        NewStatus = (byte)request.NewStatus,
                        ModifiedByUserId = 1,
                        request.Notes,

                    },base.CurrentTransaction);
            bool result = affectedRows > 0;
            if (result)
            {
                     _auditTracker.AddEntry(
             tableName: "AccountApplicationType",
             recordId: request.ApplicationId.ToString(),
             operationType: "UPDATE",
             userId: "2" );

            }

            return result;


        }






        public async Task<PagedResult<AccountApplicationListItem>> GetApplicationsPagedAsync(AccountApplicationPagedRequest request)
        {
            SetAction();

            // 1. Build dynamic components
            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                SortedBy_AccountApplication.Status => $"AA.Status {sortDir}",
                SortedBy_AccountApplication.ApplicationTypeID => $"AA.ApplicationTypeID {sortDir}",
                _ => $"AA.CreatedDate {sortDir}"
            };

            // 2. Build parameters with explicit DbTypes to prevent NotSupportedException
            var parameters = new DynamicParameters();
            parameters.Add("@Offset", (request.PageNumber - 1) * request.PageSize, DbType.Int32);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32);
            parameters.Add("@OrderBy", orderByClause, DbType.String);
            parameters.Add("@CachedTotalCount", request.CachedTotalCount, DbType.Int32, ParameterDirection.Input);
            var (whereClause, accountId, filterValue) = BuildApplicationWhereClause(request);

            // Explicitly handle nulls and types
            parameters.Add("@AccountId", accountId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@WhereClause", whereClause, DbType.String, ParameterDirection.Input);

            // Determine type for @Value based on what the filter returned
            var valueType = filterValue switch
            {
                byte => DbType.Byte,
                int => DbType.Int32,
                _ => DbType.String
            };
            parameters.Add("@Value", filterValue, valueType, ParameterDirection.Input);

            // 3. Execute via ExecuteTransientAsync for automatic transient error / retry handling
            var result = await ExecuteTransientAsync(async connection =>
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "dbo.sp_GetAccountApplicationsPaged",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    int totalCount = await multi.ReadFirstAsync<int>();
                    var data = (await multi.ReadAsync<AccountApplicationListItem>()).ToList();

                    return new PagedResult<AccountApplicationListItem>
                    {
                        Data = data,
                        TotalCount = totalCount
                    };
                }
            });
            return result;
        }
        private (string WhereClause, object? AccountId, object? FilterValue) BuildApplicationWhereClause(AccountApplicationPagedRequest request)
        {
            string where = request.AccountId.HasValue && request.AccountId.Value > 0
                ? "WHERE AA.AccountID = @AccountId"
                : "WHERE 1=1";

            object? accountId = request.AccountId.HasValue ? request.AccountId.Value : null;
            object? filterValue = null;

            if (request.FilterBy != null && !string.IsNullOrWhiteSpace(request.FilterValue))
            {
                string trimmed = request.FilterValue.Trim();
                switch (request.FilterBy.Value)
                {
                    case Filter_AccountApplication.ApplicationTypeID:
                        if (byte.TryParse(trimmed, out byte typeId)) { where += " AND AA.ApplicationTypeID = @Value"; filterValue = typeId; }
                        break;
                    case Filter_AccountApplication.Status:
                        if (byte.TryParse(trimmed, out byte statusId)) { where += " AND AA.Status = @Value"; filterValue = statusId; }
                        break;
                    case Filter_AccountApplication.AccountID:
                        if (int.TryParse(trimmed, out int accId)) { where += " AND AA.AccountID = @Value"; filterValue = accId; }
                        break;
                }
            }
            return (where, accountId, filterValue);
        }


        // =========================================================================
        // HIGH PERFORMANCE ADVANCED PAGINATION (ADO.NET Reader)
        // =========================================================================

        /*
        public async Task<PagedResult<AccountApplicationListItem>> GetApplicationsPagedAsync(
       AccountApplicationPagedRequest request)
        {
            base.SetAction();

            List<AccountApplicationListItem> records = null!;
            int totalCount = 0;

            // Invoke our generic execution handler engine
            await ExecuteApplicationReaderAsync(
                request,
                onCountCalculated: (expectedAllocationSize) =>
                {
                    // Senior-level optimization: Allocate exact list capacity up-front
                    records = new List<AccountApplicationListItem>(expectedAllocationSize);
                },
                rowProcessor: async (reader) =>
                {
                    records.Add(MapSqlReaderToApplicationResponse(reader));
                    await Task.CompletedTask; // Satisfies async signature cleanly
                },
                totalCountCallback: (count) =>
                {
                    totalCount = count;
                });

            return new PagedResult<AccountApplicationListItem>
            {
                Data = records ?? new List<AccountApplicationListItem>(0),
                TotalCount = totalCount
            };
        }

        private async Task ExecuteApplicationReaderAsync(
            AccountApplicationPagedRequest request,
            Action<int> onCountCalculated,
            Func<SqlDataReader, Task> rowProcessor,
            Action<int> totalCountCallback)
        {
            // 1. Establish strict sorting whitelists
            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                SortedBy_AccountApplication.Status => $"AA.Status {sortDir}",
                SortedBy_AccountApplication.ApplicationTypeID => $"AA.ApplicationTypeID {sortDir}",
                SortedBy_AccountApplication.CreatedDate => $"AA.CreatedDate {sortDir}",
                _ => $"AA.CreatedDate {sortDir}"
            };

            // 2. 🧠 THE MAGIC: Initialize WhereClause based on AccountApplication context
            string whereClause = "";
            object sqlAccountParam = DBNull.Value;

            if (request.AccountId.HasValue && request.AccountId.Value > 0)
            {
                // Use Case A: AccountApplication Details Page View
                whereClause = "WHERE AA.AccountID = @AccountId";
                sqlAccountParam = request.AccountId.Value;
            }
            else
            {
                // Use Case B: Global Application Grid View
                whereClause = "WHERE 1=1"; // Allows seamless appending using "AND" strings
            }

            // 3. Multi-layered dynamic grid filter parsing
            object sqlValueParam = DBNull.Value;

            if (request.FilterBy != null && !string.IsNullOrWhiteSpace(request.FilterValue))
            {
                string trimmedValue = request.FilterValue.Trim();

                switch (request.FilterBy.Value)
                {
                    case Filter_AccountApplication.ApplicationTypeID:
                        if (byte.TryParse(trimmedValue, out byte typeId))
                        {
                            whereClause += " AND AA.ApplicationTypeID = @Value";
                            sqlValueParam = typeId;
                        }
                        break;

                    case Filter_AccountApplication.Status:
                        if (byte.TryParse(trimmedValue, out byte statusId))
                        {
                            whereClause += " AND AA.Status = @Value";
                            sqlValueParam = statusId;
                        }
                        break;

                    case Filter_AccountApplication.AccountID:
                        // If filtering by AccountApplication ID explicitly from the global dashboard grid
                        if (int.TryParse(trimmedValue, out int accountId))
                        {
                            whereClause += " AND AA.AccountID = @Value";
                            sqlValueParam = accountId;
                        }
                        break;
                }
            }

            // 4. Stored Procedure invocation block
            using (var connection = new SqlConnection(base._connectionString))
            using (var command = new SqlCommand("dbo.sp_GetAccountApplicationsPaged", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Map parameters cleanly to our new unified SP signature
                command.Parameters.AddWithValue("@AccountId", sqlAccountParam);
                command.Parameters.AddWithValue("@Offset", (request.PageNumber - 1) * request.PageSize);
                command.Parameters.AddWithValue("@PageSize", request.PageSize);
                command.Parameters.AddWithValue("@OrderBy", orderByClause);
                command.Parameters.AddWithValue("@WhereClause", whereClause);
                command.Parameters.AddWithValue("@Value", sqlValueParam);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Result Set 1: Total Count processing 
                    if (await reader.ReadAsync())
                    {
                        int totalFilteredRows = reader.GetInt32(0);
                        totalCountCallback(totalFilteredRows);

                        int expectedRowsOnPage = Math.Min(request.PageSize, totalFilteredRows - ((request.PageNumber - 1) * request.PageSize));
                        expectedRowsOnPage = Math.Max(0, expectedRowsOnPage);

                        onCountCalculated(expectedRowsOnPage);
                    }

                    // Result Set 2: Paged data records stream execution
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

        private AccountApplicationListItem MapSqlReaderToApplicationResponse(SqlDataReader reader)
        {
            return new AccountApplicationListItem
            {
                ApplicationID = reader.GetInt32(reader.GetOrdinal("ApplicationID")),
                AccountID = reader.GetInt32(reader.GetOrdinal("AccountID")),
                ApplicationTypeID = reader.GetByte(reader.GetOrdinal("ApplicationTypeID")),
                Status = reader.GetByte(reader.GetOrdinal("Status")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedByUserID = reader.GetInt32(reader.GetOrdinal("CreatedByUserID")),

            };
        }

    */
    }
}
