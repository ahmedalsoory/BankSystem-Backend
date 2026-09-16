using DTOs.AccountOpeningWorkflow;
using DTOs.AccountOpeningWorkflow.interfaces;
using DTOs.interfaces;
using Microsoft.Extensions.Logging;
using RepositoryContracts.AccountWorkflowRepository;
using ServiceContract.AccountWorkflow;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Apps
{
    public class AccountWorkflowService : BaseService<AccountWorkflowService>,
        IAccountWorkflowWriteService, IAccountWorkflowReadService
    {
        private readonly IAccountWorkflowWriteRepository _writeRepo;
        private readonly IAccountWorkflowReadRepository _readRepo;

        private readonly IValidationService<IWorkflowValidationDTO> _validationService;

        public AccountWorkflowService(IAccountWorkflowWriteRepository writeRepo,
            IAccountWorkflowReadRepository readRepo,
            IValidationService<IWorkflowValidationDTO> validationService,
            ILogger<AccountWorkflowService> logger) : base(logger)
        {
            _writeRepo = writeRepo;
            _validationService = validationService;
            _readRepo = readRepo;
        }

        public async Task<OperationResult> AddWorkflowStepsAsync(WorkflowInitializationRequest request)
        {


            return await ExecuteDbOperationAsync(async () =>
            {
                return await _writeRepo.AddWorkflowStepsAsync(request);
            }, "Failed to initialize workflow steps.");


        }

        public async Task<OperationResult> CompleteStepAsync(WorkflowCompleteStatusUpdateRequest request)
        {
            List<string> errors = await _validationService.ValidateAsync(request);

            if (errors.Any())
            {
                return OperationResult.Failure(errors);
            }
            return await ExecuteDbOperationAsync(async () =>
            {
                return await _writeRepo.CompleteStepAsync(request);
            }, "Failed to update workflow step status.");

        }

        public async Task<OperationResult> IsAllStepCompleted(int ApplicationID)
        {
            return await _readRepo.IsAllStepCompleted(ApplicationID);
        }
    }
}
