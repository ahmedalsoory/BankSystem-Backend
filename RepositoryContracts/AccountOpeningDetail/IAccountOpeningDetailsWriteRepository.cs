using DTOs.AccountOpeningDetail;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountOpeningDetail
{
    public interface IAccountOpeningDetailsWriteRepository
    {
        // Save or Update a requirement (upsert logic)
        Task<OperationResult> AddDetailAsync(AccountOpeningDetailRequest request);

        // Used for modifications/corrections
     

        Task<OperationResult> VerifyDetailAsync(int detailId);

        Task<OperationResult> RejectDetailAsync(int detailId, string reason);




    }
}
