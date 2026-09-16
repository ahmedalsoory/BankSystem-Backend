using DTOs.interfaces;
using DTOs.Person;
using DTOs.Person.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RepositoryContracts.PersonRepo;
using ServiceContract.Person;
using Shared;
using Shared.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core
{
    public class PersonService : BaseService<PersonService>, IPersonWriteService
    {
        private readonly IPersonWriteRepository _personWriteRepository;
        private readonly IPersonReadTransactionRepository _personReadService;
        private readonly IValidationService<IPersonValidtionDTO> _validationService;
        private readonly IImageHandler _imageHandler;

        public PersonService(
            IPersonWriteRepository personWriteRepository,
            IPersonReadTransactionRepository personReadService,
            IValidationService<IPersonValidtionDTO> validationService,
            IImageHandler imageHandler,
            ILogger<PersonService> logger) : base(logger)
        {
            _personWriteRepository = personWriteRepository;
            _personReadService = personReadService;
            _validationService = validationService;
            _imageHandler = imageHandler;
        }

        public async Task<OperationResult<int>> AddAsync(PersonAddRequest request, IFormFile? profileImage)
        {
            List<string> errors = await _validationService.ValidateAsync(request);
            if (errors.Any()) return OperationResult<int>.Failure(errors);

            var imageResult = await _imageHandler.ProcessAsync(profileImage,null);
            if (!imageResult.Success) return OperationResult<int>.Failure(imageResult.Errors);

            request.ImagePath = imageResult.Data;

            return await ExecuteDbOperationAsync(async () =>
            {
                return await _personWriteRepository.AddAsync(request);
               
            }, "An error occurred while creating the person record.");
        }

        public async Task<OperationResult> UpdateAsync(PersonUpdateRequest request, IFormFile? profileImage)
        {
            List<string> errors = await _validationService.ValidateAsync(request, request.Id);
            if (errors.Any()) return OperationResult.Failure(errors);

            PersonResponse? existingPerson = await _personReadService.GetByIdAsync_Transaction(request.Id);

            var imageResult = await _imageHandler.ProcessAsync(profileImage, existingPerson?.ImagePath);
            if (!imageResult.Success) return OperationResult.Failure(imageResult.Errors);

            request.ImagePath = imageResult.Data;

            return await ExecuteDbOperationAsync(async () =>
            {
                return await _personWriteRepository.UpdateAsync(request);
             
            }, "An error occurred while updating the person record.");
        }
    }
}