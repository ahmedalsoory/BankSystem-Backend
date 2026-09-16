using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using DTOs.Account;
using Shared.Enums.Account;
using Shared.Enums;

namespace RepositoryContracts.AccountRepo
{
    public interface IAccountReadRepository
    {
        Task<AccountResponse?> GetByIdAsync(int id);
        Task<AccountResponse?> GetByNumberAsync(string accountNumber);
        Task<IEnumerable<AccountListItem>> GetByClientIdAsync(int clientId);
        Task<string> GetNextAccountNumberAsync();
        Task<PagedResult<AccountListItem>> GetAccountsPagedAsync(
         AccountPagedRequest request);


    }
}
