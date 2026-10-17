using DTOs.CheckLeaves;
using Microsoft.Extensions.Logging;
using RepositoryContracts.CheckLeaves;
using ServiceContract.CheckLeaves;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Banking
{
    public class CheckLeavesService : BaseService<CheckLeavesService>, ICheckLeavesWriteService
    {
        private readonly ICheckLeavesWriteRepository _checkLeavesWriteRepository;

        public CheckLeavesService(
            ICheckLeavesWriteRepository checkLeavesWriteRepository,
            ILogger<CheckLeavesService> logger) : base(logger)
        {
            _checkLeavesWriteRepository = checkLeavesWriteRepository;
        }

        public async Task<OperationResult> AddRangeAsync(AddCheckLeafs request)
        {
        

            return await _checkLeavesWriteRepository.AddRangeAsync(request);
        }
    }
}
