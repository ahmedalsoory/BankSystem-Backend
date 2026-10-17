using DTOs.CheckLeaves;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.CheckLeaves
{
    public interface ICheckLeavesWriteRepository
    {
        Task<OperationResult> AddRangeAsync(AddCheckLeafs request);
        Task<OperationResult> DisableUnusedLeavesAsync(DisableUnusedLeaves request);
    }
}
