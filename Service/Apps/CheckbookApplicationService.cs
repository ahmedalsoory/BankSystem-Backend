
using DTOs.AccountApplications;
using DTOs.CheckbookApplications;
using DTOs.CheckbookApplications.interfaces;
using DTOs.interfaces;
using Microsoft.Extensions.Logging;
using RepositoryContracts.CheckbookApplication;
using ServiceContract.Applications;
using ServiceContract.CheckbookApplication;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Apps
{
    public class CheckbookApplicationService : BaseService<CheckbookApplicationService>,
        ICheckbookApplicationWriteService
    {
        private readonly ICheckbookApplicationWriteRepository _checkbookApplicationWriteRepository;
        private readonly IAccountApplicationWriteService _accountApplicationWriteService;
  

        public CheckbookApplicationService(ICheckbookApplicationWriteRepository checkbookApplicationWriteRepository,
            IAccountApplicationWriteService accountApplicationWriteService,
         
            ILogger<CheckbookApplicationService> logger
            ) : base(logger)
        {
            _checkbookApplicationWriteRepository = checkbookApplicationWriteRepository;
            _accountApplicationWriteService = accountApplicationWriteService;
        }

   

        public async Task<OperationResult<int>> AddNewCheckbookApplicationAsync(CheckbookApplicationsAddRequest request)
        {
            OperationResult<int> result = await _accountApplicationWriteService.AddAsync(request);

            if (!result.Success) return result;

         


            return await _checkbookApplicationWriteRepository.AddNewCheckbookApplicationAsync(request, result.Data);
        }

        public async Task<OperationResult<int>> AddRenewApplicationAsync(CheckbookRenewApplicationAddRequest request)
        {
            OperationResult<int> result = await _accountApplicationWriteService.AddAsync(request);

            if (!result.Success) return result;

            return await _checkbookApplicationWriteRepository.AddRenewApplicationAsync(request, result.Data);
        }

        public async Task<OperationResult<int>> AddReplacementApplicationAsync(CheckbookReplaceApplicationAddRequest request)
        {
            OperationResult<int> result = await _accountApplicationWriteService.AddAsync(request);

            if (!result.Success) return result;

            return await _checkbookApplicationWriteRepository.AddReplacementApplicationAsync(request, result.Data);
        }
    }
}