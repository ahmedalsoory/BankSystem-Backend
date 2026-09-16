using DTOs;
using DTOs.CardType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.CardType;

namespace API.Controllers
{
  
    public class CardTypeController : MyControllerBase
    {
        private readonly ICardTypeReadServices _read;
        public CardTypeController(ICardTypeReadServices read,
            ILoggerFactory logger) : base(logger)
        {
            _read = read;
        }


        [HttpGet("GetAll")]

        public async Task<PagedResult<CardTypeResponse>> GetAll()
        {
            return await _read.GetAll().ConfigureAwait(false); ;
        }
    }
}
