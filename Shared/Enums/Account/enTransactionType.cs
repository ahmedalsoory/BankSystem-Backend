using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.Account
{
    public enum enTransactionType:byte
    {
        Deposit = 1,
        Withdraw = 2,
        Transfer = 3
    }
}
