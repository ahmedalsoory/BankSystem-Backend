using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.OnboardingApplicationTypes;
using Microsoft.Data.SqlClient;
using Repositories.Queries;
using DTOs.ApplicationWorkflowStep;
using Dapper;
using DTOs;
using Shared.Enums.WorkflowStep;
using Shared.Enums;
using System.Data;
using RepositoryContracts.ApplicationWorkflowStep;

namespace Repositories.Apps
{
    public class ApplicationWorkflowStepRepository : BaseRepository, IApplicationWorkflowStepReadRepository
    {

        public ApplicationWorkflowStepRepository(IDbConnectionProvider dbContextScope,
            Context error,
            IOptions<DbSettings> options) : base(dbContextScope, error,connectionOptions: options) { }
        public async Task<PagedResult<ApplicationWorkflowStepListItem>> GetWorkflowStepsPagedAsync(ApplicationWorkflowStepPagedRequest request)
        {
            SetAction();

            // 1. Build Sorting
            string sortDir = request.Direction == Direction.DESC ? "DESC" : "ASC";
            string orderByClause = request.SortBy switch
            {
                _ => $"AWS.StepID {sortDir}"
            };

            // 2. Build WhereClause dynamically from generic filters
            string whereClause = string.Empty;
            if (request.FilterBy.HasValue && !string.IsNullOrEmpty(request.FilterValue))
            {
                string columnName = request.FilterBy switch
                {
                    Filter_WorkflowStep.StepID => "AWS.StepID",
                    Filter_WorkflowStep.ApplicationID => "AWS.ApplicationID",
                    _ => throw new ArgumentException("Invalid filter")
                };

                whereClause = $"WHERE {columnName} = {request.FilterValue}";
            }

            // 3. Execution with CachedTotalCount parameter
            var parameters = new DynamicParameters();
            parameters.Add("@Offset", (request.PageNumber - 1) * request.PageSize, DbType.Int32);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32);
            parameters.Add("@OrderBy", orderByClause, DbType.String);
            parameters.Add("@WhereClause", whereClause, DbType.String);
            parameters.Add("@CachedTotalCount", request.CachedTotalCount, DbType.Int32, ParameterDirection.Input);

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "dbo.sp_GetApplicationWorkflowStepsPaged",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    return new PagedResult<ApplicationWorkflowStepListItem>
                    {
                        TotalCount = await multi.ReadFirstAsync<int>(),
                        Data = (await multi.ReadAsync<ApplicationWorkflowStepListItem>()).ToList()
                    };
                }
            }
        }

    }
}
