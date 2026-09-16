using DTOs.AccountOpeningDetail;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountOpeningApplications.Orchestrators
{
    public interface IAccountOpeningOrchestrator
    {
        Task<OperationResult<bool>> ProcessStatusVerificationAsync(VerifiedStatusRequest request);
    }
}
