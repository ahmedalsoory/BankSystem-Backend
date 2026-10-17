using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class CheckLeavesQ
    {
        public const string AddRangeAsync = @"
                WITH Numbers AS (
                    SELECT 0 AS n
                    UNION ALL
                    SELECT n + 1 FROM Numbers WHERE n < @NumberOfLeaves - 1
                )
                INSERT INTO Banking.CheckLeaves (CheckbookID, AccountID, CheckNumber, Status)
                SELECT 
                    @CheckbookID,
                    @AccountID,
                    @BeginCheckNumber + n,
                    @Status
                FROM Numbers
                OPTION (MAXRECURSION 0);;";

        public const string DisableUnusedLeavesAsync = @"
        UPDATE [Banking].[CheckLeaves]
        SET [Status] = @DisabledStatus
        WHERE [CheckbookID] = @OldCheckbookID 
          AND [Status] = @ActiveStatus;";
    }
}
