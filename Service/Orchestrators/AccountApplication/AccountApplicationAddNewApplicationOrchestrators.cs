using DTOs.AccountApplications;
using DTOs.ApplicationTypes;
using ServiceContract.AccountApplicationsType;
using ServiceContract.Applications;
using ServiceContract.Applications.Orchestrators;
using ServiceContract.Transaction;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Service.Orchestrators.AccountApplication
{
    public class AccountApplicationAddNewApplicationOrchestrators
        : IAccountApplicationAddNewApplicationOrchestrators
    {


        private readonly IApplicationTypeReadTransactionService _applicationTypeReadTransactionService;
        private readonly ITransactionWriteService _transactionWriteService;
        private readonly IAccountApplicationWriteService _accountApplicationWriteService;

        public AccountApplicationAddNewApplicationOrchestrators(
            IApplicationTypeReadTransactionService applicationTypeReadTransactionService, 
            ITransactionWriteService transactionWriteService,
             IAccountApplicationWriteService accountApplicationWriteService)
        {
            _applicationTypeReadTransactionService = applicationTypeReadTransactionService;
            _transactionWriteService = transactionWriteService;
            _accountApplicationWriteService = accountApplicationWriteService;
        }
        public async Task<OperationResult<int>> AddApplicationProssese(AccountApplicationAddRequest application)
        {

            OperationResult<int>app= await _accountApplicationWriteService.AddAsync(application);
            if(!app.Success) return app;

            ApplicationTypeResponse? applicationType = await
           _applicationTypeReadTransactionService.GetByIdTransactionAsync(application.ApplicationTypeID);

            if (applicationType == null)
                return OperationResult<int>.Failure("account type not exsit");

            OperationResult withdrawResult = await _transactionWriteService.WithdrawAsync(new DTOs.Transaction.WithdrawRequest()
            {
                Amount = applicationType.ApplicationFees,
                FromAccountId = application.AccountID,
                sourceType = Shared.Enums.Transaction.SourceType_Transaction.AccountApplicationFee,
                SourceId = app.Data,
                Description = $"fees for {applicationType.TypeName}"
            }, 1);

            if (!withdrawResult.Success)
                return OperationResult<int>.Failure(withdrawResult.Errors);

            return app;

        }

    }
}
