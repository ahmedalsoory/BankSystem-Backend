using DTOs.CardApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.CardApplication;
using Shared;

namespace API.Controllers
{
    
    public class CardApplicationController : MyControllerBase
    {
        private readonly ICardApplicationWriteService _cardApplicationWriteService;
        public CardApplicationController(ICardApplicationWriteService cardApplicationWriteService,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _cardApplicationWriteService = cardApplicationWriteService;
        }

        [HttpPost("Issue")]
        public async Task<OperationResult<int>> Issue([FromBody]CardApplicationAddRequest request)
        {
            return await _cardApplicationWriteService.AddNewCardApplicationAsync(request).ConfigureAwait(false); ;
        }
        [HttpPost("Renew")]
        public async Task<OperationResult<int>> Renew([FromBody] CardRenewApplicationAddRequest request)
        {
            return await _cardApplicationWriteService.AddRenewApplicationAsync(request).ConfigureAwait(false); ;
        }
        [HttpPost("Replace")]
        public async Task<OperationResult<int>> Replace([FromBody] CardReplaceApplicationAddRequest request)
        {
            return await _cardApplicationWriteService.AddReplacementApplicationAsync(request).ConfigureAwait(false); ;
        }
        [HttpPost("IssueInternational")]
        public async Task<OperationResult<int>> IssueInternational([FromBody] InternationalCardAddRequest request)
        {
            return await _cardApplicationWriteService.AddNewCardApplicationAsync(request).ConfigureAwait(false); ;
        }
    }
}
