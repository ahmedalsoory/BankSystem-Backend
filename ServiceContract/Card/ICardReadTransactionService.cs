using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Card
{
    public interface ICardReadTransactionService
    {
        Task<int?> GetCardIdByApplicationIdAsync(int applicationId);
    }
}
