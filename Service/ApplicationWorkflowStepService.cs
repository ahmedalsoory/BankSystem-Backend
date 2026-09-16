using DTOs.ApplicationWorkflowStep;
using DTOs;
using Microsoft.Extensions.Logging;
using RepositoryContracts.ApplicationWorkflowStep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.ApplicationWorkflowStep;

namespace Service
{
    public class ApplicationWorkflowStepService :BaseService<ApplicationWorkflowStepService>
        , IApplicationWorkflowStepReadService
    {

        private readonly IApplicationWorkflowStepReadRepository _read;

        public ApplicationWorkflowStepService(IApplicationWorkflowStepReadRepository read,
            ILogger <ApplicationWorkflowStepService> logger) : base(logger) 
        {
            _read = read;
        }

        public async Task<PagedResult<ApplicationWorkflowStepListItem>> GetWorkflowStepsPagedAsync
            (ApplicationWorkflowStepPagedRequest request)
        {
           return await _read.GetWorkflowStepsPagedAsync(request);
        }

    }
}
