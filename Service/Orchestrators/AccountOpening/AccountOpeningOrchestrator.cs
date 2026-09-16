using DTOs.Account;
using DTOs.AccountOpeningDetail;
using RepositoryContracts.AccountOpeningDetail;
using RepositoryContracts.AccountProducts;
using ServiceContract.Account;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.AccountWorkflow;
using ServiceContract.IAccountProducts;
using ServiceContract.Transaction;
using Shared;
using Shared.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Orchestrators.AccountOpening
{
    public class AccountOpeningOrchestrator : IAccountOpeningOrchestrator
    {
        private readonly IAccountWorkflowWriteService _workflowService;
        private readonly IAccountWorkflowReadService _workflowReadService;
        private readonly IAccountWriteService _accountService;
        private readonly IAccountOpeningApplicationsReadTransactionService _ReadTransaction;
        private readonly ITransactionWriteService _transactionWriteService;
        private readonly IAccountOpeningDetailsWriteService _detailsWriteService;
        private readonly IAccountOpeningApplicationsWriteService _accountOpeningApplicationsWriteService;
      //  private readonly ICacheVersionService _cacheVersionService;

        // Orchestrator only knows about the higher-level service interfaces
        public AccountOpeningOrchestrator(
          IAccountOpeningApplicationsWriteService accountOpeningApplicationsWriteService,
            IAccountWorkflowWriteService workflowService,
            IAccountWriteService accountService,
            IAccountWorkflowReadService workflowReadService,
            IAccountOpeningApplicationsReadTransactionService ReadTransaction,
            ITransactionWriteService transactionWriteService,
           // ICacheVersionService cacheVersionService,
            IAccountOpeningDetailsWriteService detailsWriteService
            )
        {
            _workflowService = workflowService;
            _accountService = accountService;
            _workflowReadService = workflowReadService;
            _ReadTransaction = ReadTransaction;
            _transactionWriteService = transactionWriteService;
            _detailsWriteService = detailsWriteService;
            _accountOpeningApplicationsWriteService = accountOpeningApplicationsWriteService;
        //    _cacheVersionService = cacheVersionService;
        }

        public async Task<OperationResult<bool>> ProcessStatusVerificationAsync(VerifiedStatusRequest request)
        {
            OperationResult result = await _detailsWriteService.VerifiedStatusAsync(request);
            if (!result.Success) return OperationResult<bool>.Failure(result.Errors); // Adjust based on your failure method

            var workflowResult = await _workflowService.CompleteStepAsync(request.WorkFlowData);
            if (!workflowResult.Success) return OperationResult<bool>.Failure(workflowResult.Errors);

           // _cacheVersionService.Invalidate("accountopeningapplications_paged");

            // Check if this step finishes the entire workflow
            var isReady = await _workflowReadService.IsAllStepCompleted(request.WorkFlowData.ApplicationID);

            if (isReady.Success)
            {
                var appData = await _ReadTransaction.GetByIdAsyncTransactional(request.WorkFlowData.ApplicationID);
                if (appData == null) return OperationResult<bool>.Failure("failure to return application data");
                OperationResult Completed = await _accountOpeningApplicationsWriteService.Completed(appData.ApplicationID);
                if (!Completed.Success) return OperationResult<bool>.Failure(Completed.Errors);

                var accountResult = await _accountService.AddAsync(new AccountAddRequest
                {
                    ApplicationID = appData.ApplicationID,
                    ClientID = appData.ClientID,
                    AccountType = appData.AccountType,
                    Currency = appData.Currency ?? "USD",
                    CreatedByUserID = appData.CreatedByUserID,
                });

                if (!accountResult.Success) return OperationResult<bool>.Failure(accountResult.Errors);

                OperationResult trasnResult = await _transactionWriteService.DepositAsync(new DTOs.Transaction.DepositRequest()
                {
                    Amount = appData.InitialDeposit,
                    sourceType = Shared.Enums.Transaction.SourceType_Transaction.InitialDeposit,
                    ToAccountId = accountResult.Data,
                    Description = "Opening deposit from application",
                }, 1);

                if (!trasnResult.Success) return OperationResult<bool>.Failure(trasnResult.Errors);

                //_cacheVersionService.Invalidate(
                //    "accountopeningapplications_paged",
                //    "account_paged",
                //    "transactions",
                //    $"transactions_client_{appData.ClientID}",
                //    "accountopeningdetails"
                //);

                // Workflow is fully completed! Return true.
                return OperationResult<bool>.Ok(true);
            }

            // Workflow is just moving forward, but not fully finished yet. Return false.
            return OperationResult<bool>.Ok(false);
        }
    }
}
