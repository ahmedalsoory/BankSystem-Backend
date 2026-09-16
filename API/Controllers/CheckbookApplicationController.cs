
using DTOs.CheckbookApplications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.CheckbookApplication;
using Shared;

namespace API.Controllers
{
    public class CheckbookApplicationController : MyControllerBase
    {
        private readonly ICheckbookApplicationWriteService _checkbookApplicationWriteService;

        public CheckbookApplicationController(ICheckbookApplicationWriteService checkbookApplicationWriteService,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _checkbookApplicationWriteService = checkbookApplicationWriteService;
        }

        [HttpPost("Issue")]
        public async Task<OperationResult<int>> Issue([FromBody] CheckbookApplicationsAddRequest request)
        {
            return await _checkbookApplicationWriteService.AddNewCheckbookApplicationAsync(request).ConfigureAwait(false); ;
        }

        [HttpPost("Renew")]
        public async Task<OperationResult<int>> Renew([FromBody] CheckbookRenewApplicationAddRequest request)
        {
            return await _checkbookApplicationWriteService.AddRenewApplicationAsync(request).ConfigureAwait(false); ;
        }

        [HttpPost("Replace")]
        public async Task<OperationResult<int>> Replace([FromBody] CheckbookReplaceApplicationAddRequest request)
        {
            return await _checkbookApplicationWriteService.AddReplacementApplicationAsync(request).ConfigureAwait(false); ;
        }
    }
}