using Azure.Core;
using Dapper;
using DTOs.Client;
using DTOs.Person;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Repositories.Queries;
using RepositoryContracts;
using RepositoryContracts.PersonRepo;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Repositories.Core
{
    public sealed class PersonRepository : BaseRepository,
        IPersonReadRepository, IPersonWriteRepository, IPersonReadTransactionRepository
    {
        public PersonRepository(IDbConnectionProvider dbConnection, Context error,
            IOptions<DbSettings> options, IAuditTracker auditTracker)
            : base(dbConnection, error, auditTracker,options) { }

        public async Task<OperationResult<int>> AddAsync(PersonAddRequest person)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int newId= await connection.QuerySingleAsync<int>(
                Query.Person.Add, person, transaction: CurrentTransaction);

            if (newId == 0) return OperationResult<int>.Failure("add person operaion faild");

            _auditTracker?.AddEntry(
          tableName: "Person",
          recordId: newId.ToString(),
          operationType: "Insert",
          userId: "2");

            return OperationResult<int>.Ok(newId);

        }

        public async Task< PersonResponse?> GetByIdAsync(int id)
        {
            SetAction();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<PersonResponse>(
                Query.Person.GetById, new { id });
        }

        public async Task<PersonResponse?> GetByIdAsync_Transaction(int id)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await base.BeginTransactionAsync();

            return await connection.QueryFirstOrDefaultAsync<PersonResponse>(
                Query.Person.GetById, new { id },base.CurrentTransaction);
        }

        public async Task<PersonResponse?> GetByNationalIdAsync(string nationalId)
        {
            SetAction();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<PersonResponse>(
                Query.Person.GetByNationalId, new { nationalId });
        }

        public async  Task<OperationResult> UpdateAsync(PersonUpdateRequest person)
        {
            SetAction();
            var connection = await GetConnectionAsync();
            await BeginTransactionAsync();

            int rowsAffected = await connection.ExecuteAsync(
                Query.Person.Update, person, transaction: CurrentTransaction);
            if (rowsAffected == 0)
                return OperationResult.Failure("Update person Operaion faild");


            _auditTracker?.AddEntry(
         tableName: "Person",
         recordId: person.Id.ToString(),
         operationType: "Update",
         userId: "2");


            return OperationResult.Ok();
        }
    }
}