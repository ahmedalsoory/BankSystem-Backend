using DTOs.Transaction;
using DTOs;
using Shared.Enums.Transaction;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.Transaction
{
    public interface ITransactionReadRepository
    {
  
        Task<PagedResult<ClientTransactionResponse>> GetTransactionsPagedAsync(
           TransactionScope scope,
         int? targetId, // Maps to ClientID, AccountID, or null based on your choice of scope
         TransactionPagedRequest request);
        IAsyncEnumerable<ClientTransactionResponse> StreamTransactionAsync(
            CancellationToken cancellationToken = default);
    }
}
