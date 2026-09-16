using DTOs;
using DTOs.Account;
using DTOs.Client;

using DTOs.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.Account;
using Shared.Enums.Account;
using Shared.Enums;
using DTOs.Account.interfaces;
using Shared;

namespace API.Controllers
{
    
    public sealed class AccountController : MyControllerBase
    {
        private readonly IAccountWriteService accountWrite;
        private readonly IAccountReadService accountRead;

        public AccountController(ILoggerFactory loggerFactory, IAccountReadService accountRead
            ,IAccountWriteService accountWrite) : base(loggerFactory)
        {
            this.accountRead = accountRead;
            this.accountWrite = accountWrite;
        }


        [HttpPost("create")]
        public async Task<ActionResult<OperationResult>> CreateAsync([FromBody] AccountAddRequest request
           )
            {
                OperationResult result =  await accountWrite.AddAsync(request).ConfigureAwait(false);

            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("id/{Id}")]

        public async Task<AccountResponse?> GetAccountByID(int Id
            )
        {
            return await accountRead.GetByIdAsync(Id).ConfigureAwait(false); ;
        }

        [HttpGet("client/{ClientId}")]

        public async Task<IEnumerable<AccountListItem>> GetByClientIdAsync(int ClientId)
        {
            return await accountRead.GetByClientIdAsync(ClientId).ConfigureAwait(false); ;
        }

        [HttpGet("number/{AccountNumber}")]

        public async Task<AccountResponse?> GetByNumberAsync(string AccountNumber)
        {
            return await accountRead.GetByNumberAsync(AccountNumber).ConfigureAwait(false); ;
        }

        [HttpGet("paged")]
        public async Task<PagedResult<AccountListItem>> GetAccountsPagedAsync(
           [FromQuery] AccountPagedRequest request)
        {
            return await accountRead.GetAccountsPagedAsync(request).ConfigureAwait(false);
        }
        [HttpPatch("UpdateStatus")]
        public async Task<OperationResult> UpdateStatus(AccountUpdateStatusRequest request)
        {
            return await accountWrite.UpdateStatus(request).ConfigureAwait(false);
        }


        //[HttpGet("{id}")]
        //public async ValueTask<IActionResult> Get(int id)
        //{
        //    var account = await _accountService.GetAccountDetailsAsync(id);
        //    return account != null ? Ok(account) : NotFound();
        //}

        //[HttpPost("deposit")]
        //public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        //{
        //    // Extract AdminId from JWT Token (Simplified for your audit)
        //    int adminId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        //    var result = await _accountService.ExecuteDepositAsync(request, adminId);

        //    return result.IsSuccess ? Ok(result) : BadRequest(result);
        //}

        //[HttpPost("withdraw")]
        //public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        //{
        //    int adminId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        //    var result = await _accountService.ExecuteWithdrawAsync(request, adminId);
        //    return result.IsSuccess ? Ok(result) : BadRequest(result);
        //}

        //[HttpPost("transfer")]
        //public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        //{
        //    int adminId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        //    var result = await _accountService.ExecuteTransferAsync(request, adminId);
        //    return result.IsSuccess ? Ok(result) : BadRequest(result);
        //}
    }
    }
