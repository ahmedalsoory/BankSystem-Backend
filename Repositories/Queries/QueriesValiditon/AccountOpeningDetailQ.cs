using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class AccountOpeningDetailQ
    {
        public const string CheckActiveDetail = @"
            SELECT COUNT(1) 
            FROM Apps.AccountOpeningDetails 
            WHERE ApplicationID = @ApplicationID 
            AND RequirementKey = @RequirementKey 
            AND Status != 3;";
    }
}
