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
        public async Task<byte?> GetApplicationTypeByIdAsync(int id)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            var applicationTypeId = await connection.QuerySingleOrDefaultAsync<byte?>(
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

    }
}
