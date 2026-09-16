using DTOs;
using DTOs.AccountOpeningApplication;
using DTOs.AccountOpeningApplication.interfaces;
using DTOs.AccountOpeningWorkflow;
using DTOs.interfaces;
using Microsoft.Extensions.Logging;
using RepositoryContracts.AccountOpeningApplications;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountWorkflow;
using Shared;
using Shared.Enums.AccountOpeningApplications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Apps
{
    public class AccountOpeningApplicationsService : BaseService<AccountOpeningApplicationsService>,
        IAccountOpeningApplicationsWriteService, IAccountOpeningApplicationsReadService,
        IAccountOpeningApplicationsReadTransactionService
    {
        private readonly IAccountOpeningApplicationsWriteRepository _writeRepo;
        private readonly IValidationService<IAccountOpeningValidationDTO> _validation;
        private readonly IAccountOpeningApplicationsReadTransactionRepository _readTransaction;
        private readonly IAccountWorkflowWriteService _workflowWriteService;
        private readonly IAccountOpeningApplicationsReadRepository _readRepo;

        public AccountOpeningApplicationsService(IAccountOpeningApplicationsWriteRepository writeRepo
            , IValidationService<IAccountOpeningValidationDTO> validation
            , IAccountWorkflowWriteService workflowWriteService
            , IAccountOpeningApplicationsReadTransactionRepository readTransaction,
            IAccountOpeningApplicationsReadRepository readRepo,
            ILogger<AccountOpeningApplicationsService> logger) : base(logger)
        {
            _writeRepo = writeRepo;
            _readTransaction = readTransaction;
            _validation = validation;
            _workflowWriteService = workflowWriteService;
            _readRepo = readRepo;
        }

        public async Task<OperationResult> AddApplicationAsync(AccountOpeningApplicationAddRequest request)
        {
            // 1. Validation (Cheap, non-DB operation)
            var errors = await _validation.ValidateAsync(request);
            if (errors.Any()) return OperationResult.Failure(errors);

            // 2. Orchestration: Wrap the ENTIRE logical unit in one DB Policy
            return await ExecuteDbOperationAsync(async () =>
            {
                // A. Add Application
                var appResult = await _writeRepo.AddApplicationAsync(request);
                if (!appResult.Success) return OperationResult.Failure(appResult.Errors);

                // B. Add Workflow (Call the service, but it will share the same transaction!)
                // IMPORTANT: Ensure AddWorkflowStepsAsync uses the current transaction 
                // provided by DbContextScope.
                var workflowResult = await _workflowWriteService.AddWorkflowStepsAsync(new WorkflowInitializationRequest
                {
                    ApplicationID = appResult.Data,
                    OnboardingTypeID = request.OnboardingTypeID,
                });

                if (!workflowResult.Success)
                    return OperationResult.Failure("Workflow initialization failed.");

                return OperationResult.Ok();
            }, "An error occurred while creating the account application.");
        }

        public async Task<PagedResult<AccountOpeningAppListItem>> GetAccountOpeningAppsPagedAsync(AccountOpeningPagedRequest request)
        {

            return await _readRepo.GetAccountOpeningAppsPagedAsync(request);

        }

        public async Task<AccountOpeningApplicationResponse?> GetByIdAsyncTransactional(int id)
        {
            return await _readTransaction.GetByIdAsyncTransactional(id);
        }

        public async Task<OperationResult> Completed(int applicationID)
        {
            return await _writeRepo.UpdateStatus(applicationID,(byte)AccountOpeningStatus.Completed);
        }

        public async Task<OperationResult> Cancelled(int applicationID)
        {
            return await _writeRepo.UpdateStatus(applicationID, (byte)AccountOpeningStatus.Cancelled);

        }
    }
}
