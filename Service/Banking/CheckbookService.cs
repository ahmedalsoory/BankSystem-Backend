using DTOs.Checkbook;
using Microsoft.Extensions.Logging;
using RepositoryContracts.Checkbook;
using ServiceContract.Checkbook;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Banking
{
    public class CheckbookService :BaseService<CheckbookService> ,ICheckbookWriteService
    {

        public readonly ICheckbookWriteRepository _checkbookWriteRepository;

        public CheckbookService(ICheckbookWriteRepository checkbookWriteRepository,
        ILogger<CheckbookService> logger) : base(logger) 
        {
            _checkbookWriteRepository = checkbookWriteRepository;
        }
        public async Task<OperationResult<int>> Add(CheckbookAddRequest request)
        {
            return await _checkbookWriteRepository.Add(request);
        }
    }
}
