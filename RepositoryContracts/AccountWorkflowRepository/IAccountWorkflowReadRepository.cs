using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountWorkflowRepository
{
    public interface IAccountWorkflowReadRepository
    {
        Task<OperationResult> IsAllStepCompleted(int ApplicationID);
    }
}
