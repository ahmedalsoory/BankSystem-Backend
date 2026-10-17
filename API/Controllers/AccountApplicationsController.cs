using DTOs;
using DTOs.AccountApplications;
using DTOs.AccountApplications.interfaces;
using DTOs.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.Applications;
using ServiceContract.Applications.Orchestrators;
using Shared;
using System.Diagnostics;

namespace API.Controllers
{
    public class AccountApplicationsController : MyControllerBase
    {
        private readonly IAccountApplicationReadService _readService;
        private readonly IAccountApplicationUpdateStatusOrchestrators _accountApplicationUpdateStatus;

        public AccountApplicationsController(
            ILoggerFactory loggerFactory,
            IAccountApplicationReadService readService,
            IAccountApplicationUpdateStatusOrchestrators accountApplicationUpdateStatus
          ) : base(loggerFactory)
        {
            _readService = readService;
        
            _accountApplicationUpdateStatus = accountApplicationUpdateStatus;
           
        }

        /// <summary>
        /// Retrieves a single detailed account application record.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _readService.GetByIdAsync(id).ConfigureAwait(false); ;

            if (result == null)
            {
                return NotFound(new { Message = $"Application with ID {id} was not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Handles both global system searches and account-specific history searches.
        /// Tier 3 DB validation automatically runs to check if the optional AccountId exists.
        /// </summary>
        [HttpGet]
      //  [TypeFilter(typeof(GlobalValidationFilter<AccountApplicationPagedRequest, ISimpleRequest>))]

        /*
         
         */
        public async Task<PagedResult<AccountApplicationListItem>> GetApplications([FromQuery] AccountApplicationPagedRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            // 2. Run your paged database operation (e.g., 4.5M rows evaluation)
            var result = await _readService.GetApplicationsPagedAsync(request).ConfigureAwait(false);

            // 3. Stop the clock
            stopwatch.Stop();

            // 4. Extract the exact elapsed milliseconds
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // 5. Append this time to your Response Headers or console for debugging
            Response.Headers.Add("X-Database-Execution-Time-MS", elapsedMilliseconds+"");

            Console.WriteLine($"[PERFORMANCE LOG] Querying 4.5M records took: {elapsedMilliseconds} ms");

            return result;
        }

        /// <summary>
        /// Updates an application's workflow status.
        /// Tier 1 (FluentValidation) and Tier 2 (Missing Concurrency Token check) run here.
        /// Database round-trips are bypassed because Dapper handles the atomic concurrency lookup during execution.
        /// </summary>
        [HttpPut("status")]
        //[TypeFilter(typeof(GlobalValidationFilter<AccountApplicationStatusUpdateRequest, IVersioned>))]


        public async Task<OperationResult> UpdateStatus([FromBody] AccountApplicationStatusUpdateRequest request)
        {
            return await _accountApplicationUpdateStatus.UpdateStatus(request).ConfigureAwait(false);
            
        }

    }
}
