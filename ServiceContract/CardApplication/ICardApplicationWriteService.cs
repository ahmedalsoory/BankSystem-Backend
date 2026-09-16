using DTOs.CardApplication;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.CardApplication
{
    public interface ICardApplicationWriteService
    {
        Task<OperationResult<int>> AddNewCardApplicationAsync(CardApplicationAddRequest request);

        Task<OperationResult<int>> AddReplacementApplicationAsync(CardReplaceApplicationAddRequest request);

        Task<OperationResult<int>> AddRenewApplicationAsync(CardRenewApplicationAddRequest request);
    }
}
