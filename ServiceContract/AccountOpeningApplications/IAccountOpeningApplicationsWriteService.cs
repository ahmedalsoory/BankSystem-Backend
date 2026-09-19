using DTOs.AccountOpeningApplication;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountOpeningApplications
{
    public interface IAccountOpeningApplicationsWriteService
    {
        Task<OperationResult> Cancelled(int applicationID);
        Task<OperationResult> Completed(int applicationID);
        Task<OperationResult<int>> AddApplicationAsync(AccountOpeningApplicationAddRequest request);
    }
}
