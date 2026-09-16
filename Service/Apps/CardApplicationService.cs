using DTOs.CardApplication;
using DTOs.CardApplication.interfaces;
using DTOs.interfaces;
using Microsoft.Extensions.Logging;
using RepositoryContracts.CardApplication;
using ServiceContract.Applications;
using ServiceContract.CardApplication;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Apps
{
    public class CardApplicationService : BaseService<CardApplicationService>,
        ICardApplicationWriteService
    {
        private readonly ICardApplicationWriteRepository _cardApplicationWriteRepository;
        private readonly IAccountApplicationWriteService _accountApplicationWriteService;
        private readonly IValidationService<ICardApplicationValidation> _validationService;
        public CardApplicationService(ICardApplicationWriteRepository cardApplicationWriteRepository,
            IAccountApplicationWriteService accountApplicationWriteService,
            IValidationService<ICardApplicationValidation> validationService,
        ILogger<CardApplicationService> logger
            ) : base(logger)
        {
            _cardApplicationWriteRepository = cardApplicationWriteRepository;
            _accountApplicationWriteService = accountApplicationWriteService;
            _validationService = validationService;
        }


        public async Task<OperationResult<int>> AddNewCardApplicationAsync(CardApplicationAddRequest request)
        {
            OperationResult<int> result = await _accountApplicationWriteService.AddAsync(request);

            if (!result.Success) return result;


            List<string> errors = await _validationService.ValidateAsync(request);

            if (errors.Any())
            {
                return new OperationResult<int>() { Errors = errors };
            }


            return await _cardApplicationWriteRepository.AddNewCardApplicationAsync(request, result.Data);
        }

        public async Task<OperationResult<int>> AddRenewApplicationAsync(CardRenewApplicationAddRequest request)
        {
            OperationResult<int> result = await _accountApplicationWriteService.AddAsync(request);

            if (!result.Success) return result;

            List<string> errors = await _validationService.ValidateAsync(request);

            if (errors.Any())
            {
                return new OperationResult<int>() { Errors = errors };
            }

            return await _cardApplicationWriteRepository.AddRenewApplicationAsync(request, result.Data);
        }

        public async Task<OperationResult<int>> AddReplacementApplicationAsync(CardReplaceApplicationAddRequest request)
        {
            OperationResult<int> result = await _accountApplicationWriteService.AddAsync(request);

            if (!result.Success) return result;

            List<string> errors = await _validationService.ValidateAsync(request);

            if (errors.Any())
            {
                return new OperationResult<int>() { Errors = errors };
            }

            return await _cardApplicationWriteRepository.AddReplacementApplicationAsync(request, result.Data);
        }
    }
}
