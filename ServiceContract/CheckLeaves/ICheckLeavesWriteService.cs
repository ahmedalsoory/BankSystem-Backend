using DTOs.CheckLeaves;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.CheckLeaves
{
    public interface ICheckLeavesWriteService
    {
        Task<OperationResult> AddRangeAsync(AddCheckLeafs request);
    }
}
