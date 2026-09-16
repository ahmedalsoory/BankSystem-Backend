using DTOs.AccountOpeningApplication;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountOpeningApplications
{
    public interface IAccountOpeningApplicationsReadService
    {
        Task<PagedResult<AccountOpeningAppListItem>> GetAccountOpeningAppsPagedAsync(
   AccountOpeningPagedRequest request);
    }
}
