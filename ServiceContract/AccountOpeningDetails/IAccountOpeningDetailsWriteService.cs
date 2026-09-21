using DTOs.AccountOpeningDetail;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountOpeningDetails
{
    public interface IAccountOpeningDetailsWriteService
    {
        Task<OperationResult<int>> AddDetailAsync(AccountOpeningDetailRequest request);

        // Used for modifications/corrections
        Task<OperationResult> VerifiedStatusAsync(VerifiedStatusRequest request);

        Task<OperationResult> RejectDetailAsync(int detailId, string reason);
    }
}
