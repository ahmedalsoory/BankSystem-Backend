using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class CardApplicationValidationQ
    {

        public const string IsCardOwnedByAccount = @"
   SELECT CASE WHEN EXISTS (
    SELECT 1 
    FROM [Banking].[Cards] cb
    WHERE cb.[CardID] = @OldCardId 
      AND cb.[AccountID] = @AccountId
) THEN 1 ELSE 0 END";


        public const string IsBalanceSufficient = @"
SELECT CASE 
    WHEN EXISTS (
        SELECT 1 
        FROM [Banking].[Accounts] WITH (UPDLOCK)
        WHERE [AccountID] = @AccountId 
          AND [Balance] >= (
              SELECT [MinBalanceRequired] 
              FROM [Ref].[CardTypes] 
              WHERE [CardTypeID] = @CardTypeId
          )
    ) THEN CAST(1 AS BIT)
    ELSE CAST(0 AS BIT)
END";

        public const string HasActiveCardOfType = @"
    SELECT CASE WHEN EXISTS (
        SELECT 1 
        FROM [Banking].[Cards] 
        WHERE [AccountID] = @AccountId 
          AND [CardTypeID] = @CardTypeID
          AND [StatusID] = 2 -- Adjust this status ID if your active status differs (e.g., Active)
    ) THEN 1 ELSE 0 END";
    }
}
