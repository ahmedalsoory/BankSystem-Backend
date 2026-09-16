using DTOs;
using DTOs.Account;
using Shared.Enums.Account;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Account
{
    public interface IAccountReadService
    {
        Task<AccountResponse?> GetByIdAsync(int id);
        Task<AccountResponse?> GetByNumberAsync(string accountNumber);
        Task<IEnumerable<AccountListItem>> GetByClientIdAsync(int clientId);
        Task<PagedResult<AccountListItem>> GetAccountsPagedAsync(
        AccountPagedRequest request);
    }
}
