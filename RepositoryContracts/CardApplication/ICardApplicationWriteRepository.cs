using DTOs.CardApplication;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.CardApplication
{
    public interface ICardApplicationWriteRepository
    {
        Task<OperationResult<int>> AddNewCardApplicationAsync(CardApplicationAddRequest request, int applicationId);

        Task<OperationResult<int>> AddReplacementApplicationAsync(CardReplaceApplicationAddRequest request, int applicationId);

        Task<OperationResult<int>> AddRenewApplicationAsync(CardRenewApplicationAddRequest request, int applicationId);
    }
}
