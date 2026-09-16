using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class AccountValidationQ
    {
        public const string CountAccountsForClient = @"
            SELECT COUNT(*) 
            FROM Banking.Accounts WITH (UPDLOCK, HOLDLOCK) 
            WHERE ClientID = @ClientID";
    }
}
