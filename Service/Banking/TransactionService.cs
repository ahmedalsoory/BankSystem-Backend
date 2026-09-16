using Azure.Core;
using DTOs;
using DTOs.interfaces;
using DTOs.Transaction;
using DTOs.Transaction.interfaces;
using Microsoft.Extensions.Logging;
using RepositoryContracts.AccountRepo;
using RepositoryContracts.Transaction;
using ServiceContract.CSVExportService;
using ServiceContract.Transaction;
using Shared;
using Shared.Cache;
using Shared.Enums;
using Shared.Enums.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Banking
{
    public class TransactionService : BaseService<TransactionService>,
        ITransactionWriteService, ITransactionReadService
    {
        private readonly ITransactionWriteRepository _writeRepo;
        private readonly ITransactionReadRepository _readRepo;
        private readonly IAccountLockRepository _lockRepo;
        private readonly IValidationService<ITransactionValidationDTO> _validationService;
        private readonly ICSVExportService _csvExportService;
        //  private readonly ICacheVersionService _cacheVersionService;
        public TransactionService(ITransactionReadRepository transactionRead, ITransactionWriteRepository
             transactionWrite, IAccountLockRepository accountLock,
            IValidationService<ITransactionValidationDTO> validationService,
             ICSVExportService csvExportService,
           // ICacheVersionService cacheVersionService,
            ILogger<TransactionService> logger) : base(logger)
        {
            _writeRepo = transactionWrite;
            _readRepo = transactionRead;
            _lockRepo = accountLock;
            _validationService = validationService;
            _csvExportService = csvExportService;
        //    _cacheVersionService = cacheVersionService;
        }


        public async Task<string> ExportAllTransactionsAsync(CancellationToken cancellationToken = default)
        {
            // 1. Get the database stream
            var tableStream = _readRepo.StreamTransactionAsync(cancellationToken);

            // 2. Pass to CSV/Zip service and return the path of the compressed zip file
            return await _csvExportService.ExportAndCompressToZipAsync(
                dataStream: tableStream,
                cancellationToken: cancellationToken
            );
        }

        public async Task<PagedResult<ClientTransactionResponse>> GetClientTransactionsPagedAsync
            (int clientId, TransactionPagedRequest request)
        {

         

            return await _readRepo.GetTransactionsPagedAsync(TransactionScope.ClientProfile, clientId
                ,request);
        }

        public async Task<PagedResult<ClientTransactionResponse>> GetGlobalTransactionsPagedAsync
            ( TransactionPagedRequest request)
        {
            return await _readRepo.GetTransactionsPagedAsync(TransactionScope.GlobalDashboard, null, request);
        }

        public async Task<PagedResult<ClientTransactionResponse>> GetAccountTransactionsPagedAsync
         (int AccountId, TransactionPagedRequest request)
        {
            return await _readRepo.GetTransactionsPagedAsync(TransactionScope.AccountDetails, AccountId,
                request);
        }




        public async Task<OperationResult> DepositAsync(DepositRequest request, int executedByUserId)
        {
            return  await ExecuteDbOperationAsync(async () =>
            {
                var result = await _writeRepo.DepositAsync(request, executedByUserId);
                if (!result.Success) return result;
               // _cacheVersionService.Invalidate("account_paged");
                return result;
            }, "An error occurred while processing the deposit.");
        }

        public async Task<OperationResult> WithdrawAsync(WithdrawRequest request, int executedByUserId)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                List<string> errors = await _validationService.ValidateAsync(request);
                if (errors.Any())
                {
                    return OperationResult.Failure(errors);
                }
                var result = await _writeRepo.WithdrawAsync(request, executedByUserId);
                if (!result.Success) return result;
              //  _cacheVersionService.Invalidate("account_paged");
                return result;

            }, "An error occurred while processing the withdrawal.");
        }

        public async Task<OperationResult> TransferAsync(TransferRequest request, int executedByUserId)
        {


            return await ExecuteDbOperationAsync(async () =>
            {
                int firstId = Math.Min(request.ToAccountId, request.FromAccountId);
                int SecondId = Math.Max(request.ToAccountId, request.FromAccountId);

                await _lockRepo.LockAccountForTransferAsync(firstId, SecondId);
                List<string> errors = await _validationService.ValidateAsync(request);
                if (errors.Any())
                {
                    return OperationResult.Failure(errors);
                }
                var result = await _writeRepo.TransferAsync(request, executedByUserId);
                if (!result.Success) return result;
              //  _cacheVersionService.Invalidate("account_paged");
                return result;
            }, "An error occurred while processing the transfer.");
        }
    }
}
