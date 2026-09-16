using API.Controllers;
using DTOs.Transaction;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.Transaction;
using Shared.Enums.Transaction;
using Shared.Enums;
using Service.Banking;

namespace BankSystem.Controllers
{ 
    public class TransactionsController : MyControllerBase
    {
        private readonly ITransactionWriteService _transactionWriteService;
        private readonly ITransactionReadService _transactionReadService; // 🚀 Fixed typo name

        public TransactionsController(
            ITransactionWriteService transactionWriteService,
            ITransactionReadService transactionReadService,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _transactionWriteService = transactionWriteService;
            _transactionReadService = transactionReadService;
        }


        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportTransactionsCsv(CancellationToken cancellationToken)
        {
            string zipOutputPath = string.Empty;

            try
            {
                // 1. Call application service which generates, compresses, and returns the zip file path
                zipOutputPath = await _transactionReadService.ExportAllTransactionsAsync(cancellationToken);

                // 2. Return the compressed ZIP file to the browser
                return PhysicalFile(zipOutputPath, "application/zip", "Transactions.zip");
            }
            catch (Exception)
            {
                // 3. Clean up the temp zip file if an error occurs before streaming starts
                if (!string.IsNullOrEmpty(zipOutputPath) && System.IO.File.Exists(zipOutputPath))
                {
                    System.IO.File.Delete(zipOutputPath);
                }
                throw;
            }
        }
        // =========================================================================
        // WRITE COMMANDS (Deposit, Withdraw, Transfer)
        // =========================================================================

        // URL: POST api/transactions/deposit
        [HttpPost("deposit")]
        public async Task<ActionResult<bool>> Deposit([FromBody] DepositRequest request)
        {
            // Get the Admin/User ID from the Secure Token (Hardcoded to 2 for now as per your setup)
            var result = await _transactionWriteService.DepositAsync(request, 2).ConfigureAwait(false); ;
            return Ok(result);
        }

        // URL: POST api/transactions/withdraw
        [HttpPost("withdraw")]
        public async Task<ActionResult<bool>> Withdraw([FromBody] WithdrawRequest request)
        {
            var result = await _transactionWriteService.WithdrawAsync(request, 2).ConfigureAwait(false); ;
            return Ok(result);
        }

        // URL: POST api/transactions/transfer
        [HttpPost("transfer")]
        public async Task<ActionResult<bool>> Transfer([FromBody] TransferRequest request)
        {
            var result = await _transactionWriteService.TransferAsync(request, 2).ConfigureAwait(false); ;
            return Ok(result);
        }

        // =========================================================================
        // READ QUERIES (High-Performance Paginated Feeds)
        // =========================================================================

        /// <summary>
        /// 🌍 ENDPOINT 1: Global Ledger Log Feed (Admin / Auditing Dashboard)
        /// URL: GET api/transactions?pageNumber=1&pageSize=15&sortBy=TransactionDate&direction=DESC
        /// </summary>
        [HttpGet]
        public async Task<PagedResult<ClientTransactionResponse>> GetGlobalFeed(
         [FromQuery] TransactionPagedRequest request)
        {
            return await _transactionReadService.GetGlobalTransactionsPagedAsync(request).ConfigureAwait(false); ;
                

        }

        /// <summary>
        /// 👤 ENDPOINT 2: Scoped Customer Statement Feed
        /// URL: GET api/transactions/client/45?pageNumber=1&pageSize=15&sortBy=TransactionDate&direction=DESC
        /// </summary>
        [HttpGet("client/{clientId:int}")]
        public async Task<PagedResult<ClientTransactionResponse>> GetClientFeed(
            [FromRoute] int clientId,
           [FromQuery] TransactionPagedRequest request)
        {
            var result = await _transactionReadService.GetClientTransactionsPagedAsync(
                clientId,
                request
            ).ConfigureAwait(false); ;

            return result;
        }

        [HttpGet("account/{accountId:int}")]
        public async Task<PagedResult<ClientTransactionResponse>> GetAccountFeed(
           [FromRoute] int accountId,
          [FromQuery] TransactionPagedRequest request)
        {
            var result = await _transactionReadService.GetAccountTransactionsPagedAsync(
                accountId,
              request
            ).ConfigureAwait(false); ;

            return result;
        }

    }
}