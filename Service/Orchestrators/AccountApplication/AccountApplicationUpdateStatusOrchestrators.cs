using DTOs.AccountApplications;
using RepositoryContracts.AccountApplications;
using RepositoryContracts.CheckbookApplication;
using ServiceContract.Applications;
using ServiceContract.Applications.Orchestrators;
using ServiceContract.Card;
using ServiceContract.Checkbook;
using Shared;
using Shared.Enums.AccountApplications;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Orchestrators.AccountApplication
{
    public class AccountApplicationUpdateStatusOrchestrators:
        IAccountApplicationUpdateStatusOrchestrators
    {
        private readonly IAccountApplicationWriteService _accountApplicationWriteService;
        private readonly ICardWriteService _cardWriteService;
        private readonly IAccountApplicationReadTransactionService 
            _accountApplicationReadTransactionService;

        private readonly ICardReadTransactionService _cardReadTransactionService;

        private readonly ICheckbookWriteService _checkbookWriteService;
        private readonly ICheckbookApplicationReadTransactionRepository 
            _checkbookApplicationReadTransactionRepository;
        public AccountApplicationUpdateStatusOrchestrators(IAccountApplicationWriteService
            accountApplicationWriteService, ICardWriteService cardWriteService,
            IAccountApplicationReadTransactionService 
            accountApplicationReadTransactionService, 
            ICardReadTransactionService cardReadTransactionService
            , ICheckbookWriteService checkbookWriteService,
            ICheckbookApplicationReadTransactionRepository
            checkbookApplicationReadTransactionRepository)
        {
            _accountApplicationWriteService = accountApplicationWriteService;
            _cardWriteService = cardWriteService;
            _accountApplicationReadTransactionService = accountApplicationReadTransactionService;
            _cardReadTransactionService = cardReadTransactionService;
            _checkbookWriteService = checkbookWriteService;
            _checkbookApplicationReadTransactionRepository = checkbookApplicationReadTransactionRepository;
        }

        public async Task<OperationResult>UpdateStatus(AccountApplicationStatusUpdateRequest
            request)
        {
            
            bool result = await _accountApplicationWriteService.UpdateStatusAsync(request);
            if (!result)
            {
                return OperationResult.Failure("Account application Update status filed");
            }
            if (request.NewStatus != ApplicationStatus.Approved) return OperationResult.Ok();
           
            ApplicationType? type =
                await _accountApplicationReadTransactionService.GetApplicationTypeByIdAsync(request.ApplicationId);

            if (!type.HasValue)
            {
                return OperationResult.Failure($"Account application with ID {request.ApplicationId} was not found.");
            }


            switch (type)
            {
               
                case ApplicationType.IssueLocalVisa:
                    OperationResult<int> app =
                      await _cardWriteService.IssueCardAsync(request.ApplicationId);
                    if (!app.Success)
                    {
                        return OperationResult.Failure(app.Errors);
                    }
                    break;
                case ApplicationType.ReplaceVisaCard:
                    int? oldCardIdForReplace = await _cardReadTransactionService.
                        GetCardIdByApplicationIdAsync(request.ApplicationId);
                    if (!oldCardIdForReplace.HasValue) return OperationResult.Failure("Card ID not exist");
                    OperationResult<int> replace =
                        await _cardWriteService.ReplaceCardAsync(oldCardIdForReplace.Value, request.ApplicationId);
                    if (!replace.Success) return replace;
                    
                    break;
                case ApplicationType.RenewVisaCard:
                    int? oldCardIdForRenew = await _cardReadTransactionService.
                         GetCardIdByApplicationIdAsync(request.ApplicationId);
                    if (!oldCardIdForRenew.HasValue) return OperationResult.Failure("Card ID not exist");
                    OperationResult<int> renew =
                        await _cardWriteService.RenewCardAsync(oldCardIdForRenew.Value, request.ApplicationId);
                    if (!renew.Success) return renew;
                    break;

                case ApplicationType.IssueInternationalVisa:
                    break;
                case ApplicationType.LoanRequest:
                    break;
                case ApplicationType.IssueCheckbook:
                
                    OperationResult<int> issueCheckResult = 
                        await _checkbookWriteService.IssueCheckbookAsync(request.ApplicationId);
                    if (!issueCheckResult.Success)
                    {
                        return OperationResult.Failure(issueCheckResult.Errors);
                    }
                    break;

                case ApplicationType.ReplaceCheckbook:
                    int? oldIdForReplaceCheck = await _checkbookApplicationReadTransactionRepository.GetCheckIdByApplicationIdAsync(
                    request.ApplicationId);
                    OperationResult<int> ReplaceCheckResult =
                        await _checkbookWriteService.ReplaceCheckbookAsync(oldIdForReplaceCheck.Value,request.ApplicationId);
                    if (!ReplaceCheckResult.Success)
                    {
                        return OperationResult.Failure(ReplaceCheckResult.Errors);
                    }
                    break;
                case ApplicationType.RenewCheckbook:
                    int? oldIdForRenewCheck = await _checkbookApplicationReadTransactionRepository.GetCheckIdByApplicationIdAsync(
                    request.ApplicationId);

                    OperationResult<int> RenewCheckResult =
                      await _checkbookWriteService.RenewCheckbookAsync(oldIdForRenewCheck.Value,
                      request.ApplicationId);
                    if (!RenewCheckResult.Success)
                    {
                        return OperationResult.Failure(RenewCheckResult.Errors);
                    }
                    break;
                default:
                    return OperationResult.Failure("Unkown Application type");
            }
            return OperationResult.Ok();


        }
    }
}
