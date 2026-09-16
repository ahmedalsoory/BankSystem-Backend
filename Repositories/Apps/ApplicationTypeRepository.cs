using DTOs.ApplicationTypes;
using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryContracts.AccountApplicationsType;
using Microsoft.Data.SqlClient;
using Dapper;
using Repositories.Queries;

namespace Repositories.Apps
{
    public class ApplicationTypeRepository : BaseRepository,
       IApplicationTypeReadRepository,
       IApplicationTypeWriteRepository,
        IApplicationTypeReadTransactionRepository
    {

        public ApplicationTypeRepository(
            IDbConnectionProvider dbContextScope,
            Context error, IAuditTracker auditTracker,
            IOptions<DbSettings> options) : base(dbContextScope, error, auditTracker, options)
        { 

        }

        public async Task<ApplicationTypeResponse?> GetByIdAsync(byte id)
        {
            SetAction();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<ApplicationTypeResponse>(
                Query.ApplicationType.GetById, new { id });
        }

        public async Task<IEnumerable<ApplicationTypeResponse>> GetAllAsync()
        {
            SetAction();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return await connection.QueryAsync<ApplicationTypeResponse>(
                Query.ApplicationType.GetAll);
        }

        public async Task<bool> UpdateFeesAndDescriptionAsync(ApplicationTypeUpdateRequest request)
        {
            SetAction();
            var connection =  await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            bool result = await connection.ExecuteAsync(
                Query.ApplicationType.Update, request,base.CurrentTransaction)>0;

            if (result)
            {
                        _auditTracker?.AddEntry(
                tableName: "AccountApplicationType",
                recordId: request.ApplicationTypeID.ToString(),
                operationType: "Update",
                userId: "2");

            }
                return result;
        }

        public async Task<ApplicationTypeResponse?> GetByIdTransactionAsync(byte id)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            return await connection.QueryFirstOrDefaultAsync<ApplicationTypeResponse>(
               Query.ApplicationType.GetById, new { id },base.CurrentTransaction);


        }
    }
}
