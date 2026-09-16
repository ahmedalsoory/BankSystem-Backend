using DTOs.Client;
using DTOs.Person;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.ClientRepo
{
    public interface IClientWriteRepository
    {
        Task<OperationResult> RegisterClientAsync(ClientAddRequest registerClientRequest); // Returns new Account Number
        Task<OperationResult> UpdateClientAsync(ClientUpdateRequest clientUpdateRequest);

    }
}
