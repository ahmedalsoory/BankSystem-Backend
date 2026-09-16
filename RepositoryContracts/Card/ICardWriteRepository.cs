using DTOs.Card;
using Shared;
using Shared.Enums.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.Card
{
    public interface ICardWriteRepository
    {
        Task<OperationResult<int>> AddCard(CardAddRequest request);
        Task<OperationResult> UpdateStatus(int cardId, CardStatus newStatus);
    }
}
