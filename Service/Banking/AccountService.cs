using Azure.Core;
using DTOs;
using DTOs.Account;
using DTOs.Account.interfaces;
using DTOs.interfaces;
using Microsoft.Extensions.Logging;
using RepositoryContracts.AccountRepo;
using ServiceContract.Account;
using Shared;
using Shared.Enums;
using Shared.Enums.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Banking
{
    public class AccountService : BaseService<AccountService>,
        IAccountReadService, IAccountWriteService
    {
        private readonly IAccountReadRepository _readRepo;
        private readonly IAccountWriteRepository _writeRepo;
        private readonly IValidationService<IAccountValidtionDTO> _validationService;
        public AccountService(
            IAccountReadRepository readRepo,
            IAccountWriteRepository writeRepo
            , IValidationService<IAccountValidtionDTO> validationService
            , ILogger<AccountService> logger) : base(logger)
        {
            _readRepo = readRepo;
            _writeRepo = writeRepo;
            _validationService = validationService;
        }

        // Example of a Read Operation
        public async Task<AccountResponse?> GetAccountByIdAsync(int id)
        {
            // Use .Value to access the actual repository instance
            return await _readRepo.GetByIdAsync(id);
        }


        public Task<IEnumerable<AccountResponse>> GetAllAccountsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<int>> AddAsync(AccountAddRequest account)
        {
            var validationErrors = await _validationService.ValidateAsync(account);
            if (validationErrors != null && validationErrors.Any())
            {
                return OperationResult<int>.Failure(validationErrors);
            }

            account.AccountNumber = await _readRepo.GetNextAccountNumberAsync();

            return await ExecuteDbOperationAsync(async () =>
            {
                return await _writeRepo.AddAsync(account);
            }, "An error occurred while creating your account. Please check the account details.");
        }

        public async Task<AccountResponse?> GetByIdAsync(int id)
        {
            return await _readRepo.GetByIdAsync(id);
        }

        public async Task<AccountResponse?> GetByNumberAsync(string accountNumber)
        {
            return await _readRepo.GetByNumberAsync(accountNumber);
        }

        public async Task<IEnumerable<AccountListItem>> GetByClientIdAsync(int clientId)
        {
            return await _readRepo.GetByClientIdAsync(clientId);
        }

        public async Task<PagedResult<AccountListItem>> GetAccountsPagedAsync(AccountPagedRequest request)
        {
            return await _readRepo.GetAccountsPagedAsync(request);
        }

        public async Task<OperationResult> UpdateStatus(AccountUpdateStatusRequest request)
        {
            return await _writeRepo.UpdateStatus(request);
        }
    }
}
