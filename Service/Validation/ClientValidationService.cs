using DTOs.Client;

using DTOs.interfaces;
using DTOs.Person.interfaces;
using RepositoryContracts.ClientRepo;
using ServiceContract.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation
{
    /*
    public class ClientValidationService : IValidationService<IHasClient>
    {
        private readonly IValidationService<IPersonValidtionDTO> _personRepo;

        public ClientValidationService(IValidationService<IPersonValidtionDTO> personRepo)
        {
            _personRepo = personRepo;
        }

        public async Task<List<string>> ValidateAsync(IHasClient dto, int? id = null,
            IDbTransaction? transaction = null)
        {

            return await _personRepo.ValidateAsync(dto, id,  transaction );
        }
    }
    */
}
