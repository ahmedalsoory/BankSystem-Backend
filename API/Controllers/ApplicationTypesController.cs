using DTOs;
using DTOs.ApplicationTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using ServiceContract.AccountApplicationsType;
using ServiceContract.Applications;
using Shared;

namespace API.Controllers
{

    public class ApplicationTypesController : MyControllerBase
    {
        private readonly IApplicationTypeReadService _readService;
        private readonly IApplicationTypeWriteService _writeService;

        public ApplicationTypesController(IApplicationTypeReadService readService,
         IApplicationTypeWriteService writeService,ILoggerFactory loggerFactory):base(loggerFactory)
         {
            _readService = readService;
            _writeService = writeService;
        }

        // =========================================================================
        // GET: api/applicationtypes
        // =========================================================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ApplicationTypeResponse>))]
        public async Task<PagedResult<ApplicationTypeResponse>> GetAll()
        {
            var result = await _readService.GetAllAsync().ConfigureAwait(false); ;
            return new PagedResult<ApplicationTypeResponse>()
            {
                Data = result,
                TotalCount = result.Count()
            };
        }

        // =========================================================================
        // GET: api/applicationtypes/{id}
        // =========================================================================
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationTypeResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(byte id)
        {
            var result = await _readService.GetByIdAsync(id).ConfigureAwait(false); ;
            if (result == null)
            {
                return NotFound($"Application Type with ID {id} was not found.");
            }

            return Ok(result);
        }

        // =========================================================================
        // PUT: api/applicationtypes
        // =========================================================================
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
        public async Task<ActionResult<bool>> Update([FromBody] ApplicationTypeUpdateRequest request)
        {
            // The service runs validation and updates the database via isolated channel
            bool result =  await _writeService.UpdateFeesAndDescriptionAsync(request).ConfigureAwait(false); 
            return result ? Ok(OperationResult.Ok()) : BadRequest(OperationResult.Failure(""));

        }
    }
}
