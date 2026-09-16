using DTOs.Account;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Account
{
    public interface IAccountWriteService
    {
        Task<OperationResult<int>> AddAsync(AccountAddRequest account);
        Task<OperationResult> UpdateStatus(AccountUpdateStatusRequest request);
    }
}
