
using DTOs.Account;
using Shared;


namespace RepositoryContracts.AccountRepo
{
    public interface IAccountWriteRepository
    {
        Task<OperationResult<int>> AddAsync(AccountAddRequest account);
        Task<OperationResult> UpdateStatus(AccountUpdateStatusRequest request);
    }
}

