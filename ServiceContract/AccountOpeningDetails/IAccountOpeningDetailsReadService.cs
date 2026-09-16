using DTOs;
using DTOs.AccountOpeningDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountOpeningDetails
{
    public interface IAccountOpeningDetailsReadService
    {
        Task<IEnumerable<AccountOpeningDetailResponse>> GetDetailsByApplicationIdAsync(int applicationId);
        Task<PagedResult<AccountOpeningDetailListItem>> GetDetailsPagedAsync(AccountOpeningDetailPagedRequest request);

    }
}
