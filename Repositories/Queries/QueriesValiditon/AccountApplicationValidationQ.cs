using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class AccountApplicationValidationQ
    {
        public const string CheckPendingApplication = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 
                    FROM Apps.AccountApplications 
                    WHERE AccountID = @AccountID 
                      AND ApplicationTypeID = @ApplicationTypeID 
                      AND Status = 1
                ) THEN 1 ELSE 0 END;";
    }
}
