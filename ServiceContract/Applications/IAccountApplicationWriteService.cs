using DTOs.AccountApplications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Applications
{
    public interface IAccountApplicationWriteService
    {
        Task<OperationResult<int>> AddAsync(AccountApplicationAddRequest application);
        Task<bool> UpdateStatusAsync(AccountApplicationStatusUpdateRequest request);
    }
}
