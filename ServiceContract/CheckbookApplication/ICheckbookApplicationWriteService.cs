using DTOs.CheckbookApplications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.CheckbookApplication
{
    public interface ICheckbookApplicationWriteService
    {
        Task<OperationResult<int>> AddNewCheckbookApplicationAsync
            (CheckbookApplicationsAddRequest request);
        Task<OperationResult<int>> AddRenewApplicationAsync
            (CheckbookRenewApplicationAddRequest request);
        Task<OperationResult<int>> AddReplacementApplicationAsync
            (CheckbookReplaceApplicationAddRequest request);
    }
}
