using DTOs;
using DTOs.AccountOpeningApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.AccountOpeningApplications;
using Shared;

namespace API.Controllers
{

    public class AccountOpeningApplicationsController : MyControllerBase
    {


        private readonly IAccountOpeningApplicationsWriteService _write;
        private readonly IAccountOpeningApplicationsReadService _read;
        public AccountOpeningApplicationsController(IAccountOpeningApplicationsWriteService wirte,
            IAccountOpeningApplicationsReadService read, ILoggerFactory loggerFactory) : base(loggerFactory) {
        
            _write = wirte;
            _read = read;
        }


        [HttpPost("create")]

        public async Task<OperationResult> create(AccountOpeningApplicationAddRequest request)
        {
            return await _write.AddApplicationAsync(request).ConfigureAwait(false); 
        }
        [HttpGet("paged")]
        public async Task<PagedResult<AccountOpeningAppListItem>> GetAccountApplicationPagedAsync(
            [FromQuery] AccountOpeningPagedRequest request)
        {
            return await _read.GetAccountOpeningAppsPagedAsync(request).ConfigureAwait(false); 
        }


    }
}
