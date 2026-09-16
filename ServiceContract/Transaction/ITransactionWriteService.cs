using DTOs;
using DTOs.Transaction;
using Shared.Enums.Transaction;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ServiceContract.Transaction
{
    public interface ITransactionWriteService
    {
        Task<OperationResult> DepositAsync(DepositRequest request, int executedByUserId);
        Task<OperationResult> WithdrawAsync(WithdrawRequest request, int executedByUserId);
        Task<OperationResult> TransferAsync(TransferRequest request, int executedByUserId);

    }
}
