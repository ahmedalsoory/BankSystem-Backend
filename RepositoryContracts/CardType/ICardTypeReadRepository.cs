using DTOs.CardType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.CardType
{
    public interface ICardTypeReadRepository
    {
        Task<IEnumerable<CardTypeResponse>> GetAll();
    }
}
