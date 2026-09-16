using DTOs.CardType;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.CardType
{
    public interface ICardTypeReadServices
    {
        Task<PagedResult<CardTypeResponse>> GetAll();
    }
}
