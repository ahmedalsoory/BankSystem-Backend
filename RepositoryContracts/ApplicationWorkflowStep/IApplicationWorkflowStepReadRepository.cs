using DTOs.ApplicationWorkflowStep;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.ApplicationWorkflowStep
{
    public interface IApplicationWorkflowStepReadRepository
    {
        Task<PagedResult<ApplicationWorkflowStepListItem>> GetWorkflowStepsPagedAsync
            (ApplicationWorkflowStepPagedRequest request);
        

    }
}
