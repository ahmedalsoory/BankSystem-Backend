using DTOs.AccountApplications;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountApplications
{
    public interface IAccountApplicationWriteRepository
    {
        Task<OperationResult<int>> AddAsync(AccountApplicationAddRequest application);
        Task<bool> UpdateStatusAsync(AccountApplicationStatusUpdateRequest request);
   
    }
}
