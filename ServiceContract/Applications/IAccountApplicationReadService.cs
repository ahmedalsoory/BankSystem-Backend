using DTOs.AccountApplications;
using DTOs;
using Shared.Enums.AccountApplications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Applications
{
    public interface IAccountApplicationReadService
    {
        Task<AccountApplicationResponse?> GetByIdAsync(int id);

        Task<PagedResult<AccountApplicationListItem>> GetByAccountIdAsync(
          AccountApplicationPagedRequest request);

        Task<PagedResult<AccountApplicationListItem>> GetApplicationsPagedAsync(
          AccountApplicationPagedRequest request);
    }
}
