using DTOs.AccountOpeningApplication;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountOpeningApplications
{
    public interface IAccountOpeningApplicationsWriteRepository
    {
        Task<OperationResult> UpdateStatus(int applicationID, byte status);
        Task<OperationResult<int>> AddApplicationAsync(AccountOpeningApplicationAddRequest request);
    }
}
