using API.Attributes;
using DTOs.Client;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceContract.Client;
using Shared.Enums.Client;
using API.Validation;
using DTOs.Person.interfaces;

using Shared.Enums;
using Shared;
using DTOs.Person;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public  class ClientController : MyControllerBase
    {
        private readonly IClientReadService _clientReadService;
        private readonly IClientWriteService _clientWriteService;
        public ClientController(ILoggerFactory loggerFactory,IClientReadService clientReadService
            ,IClientWriteService clientWriteService) : base(loggerFactory){ 
        
            _clientReadService = clientReadService;
            _clientWriteService = clientWriteService;
         
        }

        [HttpGet("{Id}")]

        public async Task<ClientDetailDto?> GetByClientIDAsync(int Id)
        {
            return await _clientReadService.GetByClientIDAsync(Id).ConfigureAwait(false); ;
        }


        [HttpGet("paged")]

        public async Task<PagedResult<ClientListItemDto>> GetPaged (
           [FromQuery] ClientPagedRequest request )
        {
            return  await _clientReadService.GetClientsAsync(
              request).ConfigureAwait(false); ;

            

        }
        [HttpGet("client/{accountNumber}")]
        public async Task<ClientResponse?> GetByAccountNumberAsync(string accountNumber
           )
        {
            return await _clientReadService.GetByAccountNumberAsync(accountNumber).ConfigureAwait(false); ;
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<OperationResult<int>>> CreateAsync(
    [FromForm] ClientAddRequest registerClientRequest, // ASP.NET Core will bind flat or properly named fields
    IFormFile? profileImage)
        {
          
            OperationResult<int> result = await _clientWriteService.CreateAsync(registerClientRequest
                , profileImage).ConfigureAwait(false); ;

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
        [HttpPut("update")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<OperationResult>> UpdateAsync(
        [FromForm] ClientUpdateRequest registerClientRequest,
        IFormFile? profileImage)
        {
      
            OperationResult result = await _clientWriteService.UpdateAsync(registerClientRequest
                , profileImage).ConfigureAwait(false); ;

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

    }
}
