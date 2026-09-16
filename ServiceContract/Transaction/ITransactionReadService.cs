using DTOs.Transaction;
using DTOs;
using Shared.Enums.Transaction;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Transaction
{
    public interface ITransactionReadService
    {
        Task<PagedResult<ClientTransactionResponse>> GetGlobalTransactionsPagedAsync(
        TransactionPagedRequest request);

        Task<PagedResult<ClientTransactionResponse>> GetClientTransactionsPagedAsync(
            int clientId,
             TransactionPagedRequest request);

         Task<PagedResult<ClientTransactionResponse>> GetAccountTransactionsPagedAsync
         (int AccountId, TransactionPagedRequest request);


        Task<string> ExportAllTransactionsAsync(CancellationToken cancellationToken = default);
    }
}
