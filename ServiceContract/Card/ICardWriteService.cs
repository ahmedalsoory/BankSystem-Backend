using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Card
{
    public interface ICardWriteService
    {
        Task<OperationResult<int>> IssueCardAsync(int applicationId);
        Task<OperationResult<int>> RenewCardAsync(int oldCardId, int applicationId);
        Task<OperationResult<int>> ReplaceCardAsync(int oldCardId, int applicationId);
    }
}
