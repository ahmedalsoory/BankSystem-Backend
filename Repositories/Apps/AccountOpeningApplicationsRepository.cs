using DTOs.AccountApplications;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DTOs.AccountOpeningApplication;
using RepositoryContracts.AccountWorkflowRepository;
using RepositoryContracts.AccountOpeningApplications;
using DTOs.interfaces;
using DTOs.AccountOpeningApplication.interfaces;
using Shared.Exceptions;
using DTOs.AccountOpeningWorkflow;
using Microsoft.Data.SqlClient;
using Repositories.Queries;
using Shared.Enums;
using System.Data;
using Shared.Enums.AccountOpeningApplications;
using DTOs;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace Repositories.Apps
{
    public class AccountOpeningApplicationsRepository : BaseRepository,
        IAccountOpeningApplicationsWriteRepository,
        IAccountOpeningApplicationsReadTransactionRepository,
        IAccountOpeningApplicationsReadRepository
    {

        private readonly IAccountWorkflowWriteRepository _accountWorkflowWrite;
        private readonly IValidationService<IAccountOpeningValidationDTO> _validation;

        public AccountOpeningApplicationsRepository(IDbConnectionProvider provider,
            Context error, IAccountWorkflowWriteRepository accountWorkflowWrite
            , IOptions<DbSettings> options, IAuditTracker auditTracker
            , IValidationService<IAccountOpeningValidationDTO> validation) : base(provider, error,
                auditTracker, options)
        {
            _accountWorkflowWrite = accountWorkflowWrite;
            _validation = validation;

        }

        public async Task<OperationResult<int>> AddApplicationAsync(AccountOpeningApplicationAddRequest request)
        {
            SetAction();

            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            try
            {
                // Clean, readable call to the registry
                int appId = await connection.ExecuteScalarAsync<int>(
                    Query.AccountOpeningApplication.AddApplication,
                    new
                    {
                        request.ClientID,
                        request.OnboardingTypeID,
                        request.CreatedByUserID,
                        request.AccountType
                    },
                    CurrentTransaction);
                if (appId>0)
                {
                    _auditTracker?.AddEntry(
            tableName: "AccountOpeningApplications",
            recordId:appId.ToString(),
            operationType: "Insert",
            userId: "2");

                }

                return OperationResult<int>.Ok(appId);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return OperationResult<int>.Failure("An active application already exists for this client.");
            }
        }

        public async Task<AccountOpeningApplicationResponse?> GetByIdAsync(int id)
        {
            SetAction();
            // Uses the standardized HUP pattern

            return await ExecuteTransientAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<AccountOpeningApplicationResponse?>(
                    Query.AccountApplication.GetById,
                    new { id }
                );
            });

        }

        public async Task<AccountOpeningApplicationResponse?> GetByIdAsyncTransactional(int ApplicationID)
        {
            SetAction();
            // Uses the standardized HUP pattern
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            return await connection.QueryFirstOrDefaultAsync<AccountOpeningApplicationResponse>(
                Query.AccountOpeningApplication.GetByID, new { ApplicationID }, CurrentTransaction);

        }

        public async Task<OperationResult> UpdateStatus(int applicationID , byte status)
        {
            SetAction();
            // Uses the standardized HUP pattern
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();


            // Assuming you have a helper method or Dapper extension like ExecuteAsync available in your base class/repository
            var affectedRows = await connection.ExecuteAsync(
            Query.AccountOpeningApplication.UpdateStatus,
                new { ApplicationID = applicationID, Status = status },
                base.CurrentTransaction
            );

            if (affectedRows == 0)
            {
                return OperationResult.Failure("Application not found or status update failed.");
            }
            _auditTracker?.AddEntry(
       tableName: "AccountOpeningApplications",
       recordId: applicationID.ToString(),
       operationType: "Update",
       userId: "2");
            return OperationResult.Ok();

        }


        public async Task<PagedResult<AccountOpeningAppListItem>> GetAccountOpeningAppsPagedAsync(AccountOpeningPagedRequest request)
        {
            SetAction();

            // 1. Build your dynamic components
            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                SortedBy_AccountOpening.CreatedDate => $"AO.CreatedDate {sortDir}",
                _ => $"AO.CreatedDate {sortDir}"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@Offset", (request.PageNumber - 1) * request.PageSize, DbType.Int32);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32);
            parameters.Add("@OrderBy", orderByClause, DbType.String);
            parameters.Add("@WhereClause", BuildWhereClause(request, parameters), DbType.String);
            parameters.Add("@CachedTotalCount", request.CachedTotalCount, DbType.Int32, ParameterDirection.Input);

            // 2. Execute via Dapper
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "dbo.sp_GetAccountOpeningAppsPaged",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    // 3. Read the results in the exact same order as your SP
                    int totalCount = await multi.ReadFirstAsync<int>();
                    var data = (await multi.ReadAsync<AccountOpeningAppListItem>()).ToList();

                    return new PagedResult<AccountOpeningAppListItem>
                    {
                        Data = data,
                        TotalCount = totalCount
                    };
                }
            }
        }
        // Helper to keep the main method clean
        private string BuildWhereClause(AccountOpeningPagedRequest request, DynamicParameters p)
        {
            string where = "WHERE 1=1";
            if (request.FilterBy != null && !string.IsNullOrWhiteSpace(request.FilterValue))
            {
                string trimmedValue = request.FilterValue.Trim();
                if (request.FilterBy.Value == Filter_AccountOpening.ClientID && int.TryParse(trimmedValue, out int cid))
                {
                    where += " AND AO.ClientID = @Value";
                    p.Add("@Value", cid);
                }
                else if (request.FilterBy.Value == Filter_AccountOpening.ApplicationID && int.TryParse(trimmedValue, out int aid))
                {
                    where += " AND AO.ApplicationID = @Value";
                    p.Add("@Value", aid);
                }
            }
            return where;
        }


        /*
        public async Task<PagedResult<AccountOpeningAppListItem>> GetAccountOpeningAppsPagedAsync(
    AccountOpeningPagedRequest request)
        {
            base.SetAction();

            List<AccountOpeningAppListItem> records = null!;
            int totalCount = 0;

            // Invoke the engine specifically for AccountOpening
            await ExecuteAccountOpeningReaderAsync(
                request,
                onCountCalculated: (expectedAllocationSize) =>
                {
                    records = new List<AccountOpeningAppListItem>(expectedAllocationSize);
                },
                rowProcessor: async (reader) =>
                {
                    records.Add(MapSqlReaderToOpeningApp(reader));
                    await Task.CompletedTask;
                },
                totalCountCallback: (count) =>
                {
                    totalCount = count;
                });

            return new PagedResult<AccountOpeningAppListItem>
            {
                Data = records ?? new List<AccountOpeningAppListItem>(0),
                TotalCount = totalCount
            };
        }


        private async Task ExecuteAccountOpeningReaderAsync(
      AccountOpeningPagedRequest request,
      Action<int> onCountCalculated,
      Func<SqlDataReader, Task> rowProcessor,
      Action<int> totalCountCallback)
        {
            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                SortedBy_AccountOpening.CreatedDate => $"AO.CreatedDate {sortDir}",
                _ => $"AO.CreatedDate {sortDir}"
            };

            string whereClause = "WHERE 1=1";
            object sqlValueParam = DBNull.Value;

            if (request.FilterBy != null && !string.IsNullOrWhiteSpace(request.FilterValue))
            {
                string trimmedValue = request.FilterValue.Trim();
                switch (request.FilterBy.Value)
                {
                    case Filter_AccountOpening.ClientID:
                        if (int.TryParse(trimmedValue, out int clientId))
                        {
                            whereClause += " AND AO.ClientID = @Value";
                            sqlValueParam = clientId;
                        }
                        break;
                    case Filter_AccountOpening.ApplicationID:
                        if (int.TryParse(trimmedValue, out int appId))
                        {
                            whereClause += " AND AO.ApplicationID = @Value";
                            sqlValueParam = appId;
                        }
                        break;
                }
            }

            using (var connection = new SqlConnection(base._connectionString)) 
            using (var command = new SqlCommand("dbo.sp_GetAccountOpeningAppsPaged", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Offset", (request.PageNumber - 1) * request.PageSize);
                command.Parameters.AddWithValue("@PageSize", request.PageSize);
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
                        int expectedRowsOnPage = Math.Max(0, Math.Min(request.PageSize, totalFilteredRows - ((request.PageNumber - 1) * request.PageSize)));
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


        private AccountOpeningAppListItem MapSqlReaderToOpeningApp(SqlDataReader reader)
        {
            return new AccountOpeningAppListItem
            {
                ApplicationID = reader.GetInt32(reader.GetOrdinal("ApplicationID")),
                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                Status = reader.GetByte(reader.GetOrdinal("Status")),
               
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                Currency = reader.IsDBNull(reader.GetOrdinal("Currency")) ? null : reader.GetString(reader.GetOrdinal("Currency"))
            };
        }


        */
    }

}

