using DTOs.AccountApplications;
using DTOs;
using RepositoryContracts.AccountApplications;
using ServiceContract.Applications;
using Shared.Enums.AccountApplications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Shared;
using DTOs.AccountApplications.interfaces;
using DTOs.interfaces;
using Microsoft.Extensions.Logging;
using Shared.Enums.AccountApplicationsType;
using ServiceContract.AccountApplicationsType;
using DTOs.ApplicationTypes;
using ServiceContract.Transaction;

namespace Service.Apps
{
    public class AccountApplicationService : BaseService<AccountApplicationService>,
        IAccountApplicationReadService, IAccountApplicationWriteService
        , IAccountApplicationReadTransactionService

    {
        private readonly IAccountApplicationReadRepository _readRepo;
        private readonly IAccountApplicationWriteRepository _writeRepo;
        private readonly IAccountApplicationReadTransactionRepository _readTransactionRepo;


        private readonly IValidationService<IAccountApplicationValidationDTO> _validation;
        public AccountApplicationService(
            IAccountApplicationReadRepository readRepo,
            IAccountApplicationWriteRepository writeRepo,
            IAccountApplicationReadTransactionRepository readTransactionRepo,
            IValidationService<IAccountApplicationValidationDTO> validation
            , ILogger<AccountApplicationService> logger) : base(logger)
        {
            _readRepo = readRepo;
            _writeRepo = writeRepo;
            _readTransactionRepo = readTransactionRepo;
            _validation = validation;
          
        }

        // =========================================================================
        // READ SERVICE OPERATIONS
        // =========================================================================

        public async Task<ApplicationType> GetApplicationTypeByIdAsync(int id)
        {
            return (ApplicationType)await _readTransactionRepo.GetApplicationTypeByIdAsync(id);
        }

        public async Task<AccountApplicationResponse?> GetByIdAsync(int id)
        {
            // Pulls full details including Notes, Audit IDs, and RowVersion for the details view
            return await _readRepo.GetByIdAsync(id);
        }

        public async Task<PagedResult<AccountApplicationListItem>> GetByAccountIdAsync(
         AccountApplicationPagedRequest request)
        {
            // Pulls the performance-optimized list array bounded to a single Account ID grid history
            return await _readRepo.GetApplicationsPagedAsync(
               request);
        }

        public async Task<PagedResult<AccountApplicationListItem>> GetApplicationsPagedAsync(
      AccountApplicationPagedRequest request)
        {
            // Back-office global administration workbench lookups table engine
            return await _readRepo.GetApplicationsPagedAsync(
                request);
        }

        // =========================================================================
        // WRITE SERVICE OPERATIONS
        // =========================================================================

        public async Task<OperationResult<int>> AddAsync(AccountApplicationAddRequest application)
        {
            // Business logic processing execution point
            

            List<string> errors = await _validation.ValidateAsync(application);
            if (errors.Any()) return OperationResult<int>.Failure(errors);

       


            return await ExecuteDbOperationAsync(() => _writeRepo.AddAsync(application)
            , "A pending application for this account type already exists.");
        }

        public async Task<bool> UpdateStatusAsync(
            AccountApplicationStatusUpdateRequest request)
        {

            return await _writeRepo.UpdateStatusAsync(request);
        }
    }
}
