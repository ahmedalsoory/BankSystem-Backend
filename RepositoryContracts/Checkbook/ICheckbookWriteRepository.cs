using DTOs.Checkbook;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.Checkbook
{
    public interface ICheckbookWriteRepository
    {
        Task<OperationResult<int>> Add(CheckbookAddRequest request);
    }
}
