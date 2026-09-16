using Dapper;
using DTOs.interfaces;
using DTOs.Person.interfaces;
using Microsoft.Identity.Client;
using Repositories.Queries.QueriesValiditon;
using RepositoryContracts;
using RepositoryContracts.ClientRepo;
using RepositoryContracts.PersonRepo;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Validations
{
    public class PersonValidationRepository : BaseRepository, IValidationService<IPersonValidtionDTO>
    {
        public PersonValidationRepository(IDbConnectionProvider connection, Context error)
            : base(connection, error) { }

        public async Task<List<string>> GetConflictReasonsAsync(string nationalId, string email, string phone, int? personId = null)
        {
            base.SetAction();
            var connection = await base.GetConnectionAsync();
            await base.BeginTransactionAsync();

            var results = await connection.QueryAsync<string>(
                QueryValidtion.PersonValidation.CheckConflicts,
                new { NationalId = nationalId, Email = email, Phone = phone, PersonId = personId }
                ,base.CurrentTransaction
            );

            return results.ToList();
        }
        async Task<List<string>> IValidationService<IPersonValidtionDTO>.ValidateAsync(IPersonValidtionDTO dto, int? id)
        {
           
            return await GetConflictReasonsAsync(dto.NationalId, dto.Email, dto.Phone, id);
        }
    }
}