using DTOs.CheckbookApplications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.CheckbookApplication
{
    public interface ICheckbookApplicationWriteRepository
    {
        Task<OperationResult<int>> AddNewCheckbookApplicationAsync
            (CheckbookApplicationsAddRequest request, int applicationId);

         Task<OperationResult<int>> AddRenewApplicationAsync
            (CheckbookRenewApplicationAddRequest request, int applicationId);

        Task<OperationResult<int>> AddReplacementApplicationAsync
            (CheckbookReplaceApplicationAddRequest request, int applicationId);
    }
}
