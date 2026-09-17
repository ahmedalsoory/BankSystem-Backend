using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.CheckbookApplications;
using Dapper;
using Repositories.Queries;
using RepositoryContracts.CheckbookApplication;
using DTOs.Checkbook;


namespace Repositories.Apps
{
    public class CheckbookApplicationRepository : BaseRepository, 
        ICheckbookApplicationWriteRepository, ICheckbookApplicationReadTransactionRepository
    {
        public CheckbookApplicationRepository(IDbContextScope dbContextScope, Context error
            , IAuditTracker auditTracker) : base(dbContextScope, error, auditTracker)
        {

        }


        public async Task<CheckbookDataForAddCheckbook> GetDataForAddCheckbook(int ApplicationID)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            CheckbookDataForAddCheckbook Data = await connection.QuerySingleAsync<CheckbookDataForAddCheckbook>(
            Query.CheckbookApplication.GetDataForAddCheckbook,
            new { ApplicationID },
            transaction: base.CurrentTransaction
        );
            return Data;
        }

        public async Task<OperationResult<int>> AddNewCheckbookApplicationAsync(CheckbookApplicationsAddRequest request, int applicationId)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int newId = await connection.ExecuteScalarAsync<int>(Query.CheckbookApplication.InsertNewCheckbookApplication, new
            {
                ApplicationId = applicationId,
                request.NumberOfLeaves,
                request.IsUrgentProcessing,
                request.DeliveryMethod
            }, transaction: CurrentTransaction);

            if (newId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
             tableName: "CheckbookApplication",
             recordId: newId.ToString(),
             operationType: "Insert",
             userId: "2");



            return OperationResult<int>.Ok(newId);
        }

        public async Task<OperationResult<int>> AddRenewApplicationAsync(CheckbookRenewApplicationAddRequest request, int applicationId)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int newId = await connection.ExecuteScalarAsync<int>(Query.CheckbookApplication.InsertRenewApplication, new
            {
                ApplicationId = applicationId,
                request.NumberOfLeaves,
                request.IsUrgentProcessing,
                request.DeliveryMethod,
                OldCheckbookID = request.OldCheckbookID
            }, transaction: CurrentTransaction);

            if (newId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
             tableName: "CheckbookApplication",
             recordId: newId.ToString(),
             operationType: "Insert",
             userId: "2");



            return OperationResult<int>.Ok(newId);
        }

        public async Task<OperationResult<int>> AddReplacementApplicationAsync(CheckbookReplaceApplicationAddRequest request, int applicationId)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int newId = await connection.ExecuteScalarAsync<int>(Query.CheckbookApplication.InsertReplaceApplication, new
            {
                ApplicationId = applicationId,
                request.NumberOfLeaves,
                request.IsUrgentProcessing,
                request.DeliveryMethod,
                OldCheckbookID = request.OldCheckbookID
            }, transaction: CurrentTransaction);

            if (newId == 0) return OperationResult<int>.Failure("add card operaion faild");
            _auditTracker?.AddEntry(
             tableName: "CheckbookApplication",
             recordId: newId.ToString(),
             operationType: "Insert",
             userId: "2");


            return OperationResult<int>.Ok(newId);
        }
    }
}