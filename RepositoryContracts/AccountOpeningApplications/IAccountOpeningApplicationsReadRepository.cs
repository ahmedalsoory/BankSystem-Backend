using DTOs;
using DTOs.AccountOpeningApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountOpeningApplications
{
    public interface IAccountOpeningApplicationsReadRepository
    {
         Task<AccountOpeningApplicationResponse?> GetByIdAsync(int id);
        Task<PagedResult<AccountOpeningAppListItem>> GetAccountOpeningAppsPagedAsync(
    AccountOpeningPagedRequest request);
    }
}
