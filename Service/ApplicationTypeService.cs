using DTOs.ApplicationTypes;
using FluentValidation;
using RepositoryContracts.AccountApplicationsType;
using ServiceContract.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ApplicationTypeService : IApplicationTypeReadService,
        IApplicationTypeWriteService, IApplicationTypeReadTransactionService
    {
        private readonly IApplicationTypeReadRepository _readRepository;
        private readonly IApplicationTypeWriteRepository _writeRepository;
        private readonly IApplicationTypeReadTransactionRepository _readTransactionRepository;

        public ApplicationTypeService(
            IApplicationTypeReadRepository readRepository,
            IApplicationTypeWriteRepository writeRepository,
            IApplicationTypeReadTransactionRepository readTransactionRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _readTransactionRepository = readTransactionRepository;
        }

        public async Task<IEnumerable<ApplicationTypeResponse>> GetAllAsync()
        {
            return await _readRepository.GetAllAsync();
        }

        public async Task<ApplicationTypeResponse?> GetByIdAsync(byte id)
        {
            return await _readRepository.GetByIdAsync(id);
        }

        public async Task<ApplicationTypeResponse?> GetByIdTransactionAsync(byte id)
        {
            return await _readTransactionRepository.GetByIdTransactionAsync(id);
        }

        public async Task<bool> UpdateFeesAndDescriptionAsync(ApplicationTypeUpdateRequest request)
        {
            return await _writeRepository.UpdateFeesAndDescriptionAsync(request);
        }
    }
}
