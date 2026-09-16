using DTOs.AccountApplications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Applications.Orchestrators
{
    public interface IAccountApplicationAddNewApplicationOrchestrators
    {
        Task<OperationResult<int>> AddApplicationProssese(AccountApplicationAddRequest application);
    }
}
