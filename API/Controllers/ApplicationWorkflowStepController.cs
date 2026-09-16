using DTOs;
using DTOs.ApplicationWorkflowStep;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.ApplicationWorkflowStep;

namespace API.Controllers
{

    public class ApplicationWorkflowStepController : MyControllerBase
    {
        private readonly IApplicationWorkflowStepReadService _read;
        public ApplicationWorkflowStepController(IApplicationWorkflowStepReadService read,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _read = read;
        }

        //  public async Task<IActionResult> GetApplications([FromQuery] AccountApplicationPagedRequest request)
        [HttpGet("paged")]

        public async Task<PagedResult<ApplicationWorkflowStepListItem>> GetAll([FromQuery]ApplicationWorkflowStepPagedRequest request)
        {
            return await _read.GetWorkflowStepsPagedAsync(request).ConfigureAwait(false); ;
        }

    }
}
