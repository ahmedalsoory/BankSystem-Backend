using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class TransactionValidationQ
    {
        public const string IsBalanceHasWithdrawAmount = @"
             SELECT 1 
             FROM Banking.Accounts 
             WHERE AccountID = @AccountId AND Balance >= @Amount";
    }
}
