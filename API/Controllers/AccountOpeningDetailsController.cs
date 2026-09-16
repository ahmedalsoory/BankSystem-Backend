using DTOs;
using DTOs.AccountOpeningDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.AccountOpeningDetails;
using Shared;
using Shared.Enums.AccountOpeningDetails;
using Shared.Enums;
using ServiceContract.AccountOpeningApplications.Orchestrators;

namespace API.Controllers
{

    public class AccountOpeningDetailsController : MyControllerBase
    {
        private readonly IAccountOpeningDetailsReadService _readService;
        private readonly IAccountOpeningDetailsWriteService _writeService;
        private readonly IAccountOpeningOrchestrator _openingOrchestrator;
        public AccountOpeningDetailsController(IAccountOpeningOrchestrator openingOrchestrator,
            IAccountOpeningDetailsReadService readService,
            IAccountOpeningDetailsWriteService writeService,ILoggerFactory logger) : base(logger) 
        {
            _readService = readService;
            _writeService = writeService;
            _openingOrchestrator = openingOrchestrator;
        }

        [HttpGet("{applicationId}")]
        public async Task<ActionResult<IEnumerable<AccountOpeningDetailResponse>>> GetByApplicationId(int applicationId)
        {
            var result = await _readService.GetDetailsByApplicationIdAsync(applicationId).ConfigureAwait(false); ;
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> AddDetail([FromBody] AccountOpeningDetailRequest request)
        {
            var result = await _writeService.AddDetailAsync(request).ConfigureAwait(false); ;
            return result.Success ? Ok(result) : BadRequest(result);
        }



        [HttpPatch("verify")]
        public async Task<ActionResult<OperationResult<bool>>> UpdateStatus([FromBody] VerifiedStatusRequest request)
        {
            var result = await _openingOrchestrator.ProcessStatusVerificationAsync(request).ConfigureAwait(false); ;
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPatch("reject/{detailId}")]
        public async Task<ActionResult<OperationResult>> RejectStatus(int detailId, [FromBody] RejectRequest request)
        {
            // Ensure the reason is captured
            if (string.IsNullOrWhiteSpace(request?.Reason))
                return BadRequest(OperationResult.Failure("Rejection reason is required."));

            var result = await _writeService.RejectDetailAsync(detailId, request.Reason).ConfigureAwait(false); ;
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("paged")]
     
        public async Task<PagedResult<AccountOpeningDetailListItem>> GetDetailsPagedAsync(
          [FromQuery] AccountOpeningDetailPagedRequest request)
        {
           


            return await _readService.GetDetailsPagedAsync(request).ConfigureAwait(false); ;
        }

    }
}
