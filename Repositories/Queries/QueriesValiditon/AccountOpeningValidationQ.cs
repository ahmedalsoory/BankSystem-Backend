using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class AccountOpeningValidationQ
    {
        public const string CheckActiveApplications = @"
            SELECT COUNT(1) 
            FROM Apps.AccountOpeningApplications 
            WHERE ClientID = @ClientID AND Status = 1;";
    }
}
