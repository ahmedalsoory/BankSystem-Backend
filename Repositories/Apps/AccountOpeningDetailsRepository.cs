using RepositoryContracts.AccountWorkflowRepository;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryContracts.AccountOpeningDetail;
using DTOs.AccountOpeningDetail;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Shared.Enums.AccountOpeningDetails;
using DTOs.AccountOpeningWorkflow;
using DTOs.AccountOpeningDetail.interfaces;
using DTOs.interfaces;
using Repositories.Queries;
using DTOs;
using Shared.Enums;
using System.Data;
using Azure.Core;

namespace Repositories.Apps
{

    public class AccountOpeningDetailsRepository : BaseRepository,
      IAccountOpeningDetailsReadRepository, IAccountOpeningDetailsWriteRepository
    {
        public AccountOpeningDetailsRepository(IDbConnectionProvider provider, Context error
            , IOptions<DbSettings> options, IAuditTracker auditTracker)
            : base(provider, error, auditTracker, options) 
        {
           
        }

        public async Task<OperationResult> AddDetailAsync(AccountOpeningDetailRequest request)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            var affectedRows = await connection.ExecuteAsync(
                Query.AccountOpeningDetail.Add, request, CurrentTransaction);

           if(affectedRows ==0) 
                return OperationResult.Failure("Failed to add detail.");
            _auditTracker?.AddEntry(
     tableName: "AccountOpeningDetail",
     recordId: request.ApplicationID.ToString(),
     operationType: "Update",
     userId: "2");


            return OperationResult.Ok();
        }

        public async Task<IEnumerable<AccountOpeningDetailResponse>> GetDetailsByApplicationIdAsync(int applicationId)
        {
            SetAction();
            // Simple Read: Using connection directly
            var connection = await GetConnectionAsync();

            return await connection.QueryAsync<AccountOpeningDetailResponse>(
                Query.AccountOpeningDetail.GetByApplicationId, new { ApplicationID = applicationId });
        }



        public async Task<OperationResult> VerifyDetailAsync(int detailId)
        {
            SetAction();
            // Verification logic: Set status to 'Verified' (e.g., 2)
            return await UpdateDetailStatusAsync(detailId, DetailStatus.Verified);
        }

        // 2. Rejection Function
        public async Task<OperationResult> RejectDetailAsync(int detailId, string reason)
        {
            SetAction();
            if (string.IsNullOrWhiteSpace(reason))
                return OperationResult.Failure("Rejection reason is required.");

            // Rejection logic: Set status to 'Rejected' (e.g., 3) and save the reason
            // You might also need to update a 'RejectionReason' column here
            return await UpdateDetailStatusAsync(detailId, DetailStatus.Rejected, reason);
        }


        private async Task<OperationResult> UpdateDetailStatusAsync(int detailId, DetailStatus status, string? reason = null)
        {
            var obj = new
            {
                DetailID = detailId,
                NewStatus = (byte)status,
                RejectionReason = reason
            };

            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            // Use a specific query that handles the status/reason update
            var affectedRows = await connection.ExecuteAsync(
                Query.AccountOpeningDetail.UpdateStatus, obj, CurrentTransaction);

            if(affectedRows==0)
                return OperationResult.Failure("Update failed."); 

            _auditTracker?.AddEntry(
            tableName: "AccountOpeningDetail",
            recordId: detailId.ToString(),
            operationType: "Update",
            userId: "2");

            return OperationResult.Ok(); 
        }

        public async Task<PagedResult<AccountOpeningDetailListItem>> GetDetailsPagedAsync(AccountOpeningDetailPagedRequest request)
        {
            SetAction();

            // 1. Build Sorting
            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                SortedBy_AccountOpeningDetail.ApplicationID => $"AOD.ApplicationID {sortDir}",
                _ => $"AOD.DetailID {sortDir}"
            };

            // 2. Build WhereClause dynamically
            string whereClause = string.Empty;
            if (request.FilterBy.HasValue && !string.IsNullOrEmpty(request.FilterValue))
            {
                string columnName = request.FilterBy switch
                {
                    Filter_AccountOpeningDetail.DetailID => "AOD.DetailID",
                    Filter_AccountOpeningDetail.ApplicationID => "AOD.ApplicationID",
                    _ => throw new ArgumentException("Invalid filter")
                };

                whereClause = $"WHERE {columnName} = {request.FilterValue}";
            }

            // 3. Parameters for Stored Procedure
            var parameters = new DynamicParameters();
            parameters.Add("@Offset", (request.PageNumber - 1) * request.PageSize, DbType.Int32);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32);
            parameters.Add("@OrderBy", orderByClause, DbType.String);
            parameters.Add("@WhereClause", whereClause, DbType.String);
            parameters.Add("@CachedTotalCount", request.CachedTotalCount, DbType.Int32, ParameterDirection.Input);

            // 4. Execute via Dapper
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "dbo.sp_GetAccountOpeningDetailsPaged",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    int totalCount = await multi.ReadFirstAsync<int>();
                    var data = (await multi.ReadAsync<AccountOpeningDetailListItem>()).ToList();

                    return new PagedResult<AccountOpeningDetailListItem>
                    {
                        Data = data,
                        TotalCount = totalCount
                    };
                }
            }
        }

    }
}
