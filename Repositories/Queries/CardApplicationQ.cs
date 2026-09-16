using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class CardApplicationQ
    {
        public const string InsertNewCardApplication = @"
    INSERT INTO [Apps].[CardApplications] (
        [ApplicationID],
        [CardHolderName], 
        [IsInternationalEnabled], 
        [CardTypeID]
    ) 
    OUTPUT INSERTED.ApplicationID 
    SELECT 
        @ApplicationId,
        (p.FirstName + ' ' + p.LastName),
        @IsInternationalEnabled,
        @CardTypeID
    FROM [Banking].[Accounts] a
    INNER JOIN [Core].[Clients] c ON a.ClientId = c.PersonId
    INNER JOIN [Core].[Persons] p ON c.PersonId = p.Id
    WHERE a.AccountID = @AccountId";

        public const string InsertReplacementApplication = @"
INSERT INTO [Apps].[CardApplications] (
    [ApplicationID], -- 👈 Add this column
    [CardHolderName], 
    [IsInternationalEnabled], 
    [CardTypeID], 
    [OldCardId]
) 
OUTPUT INSERTED.ApplicationID 
SELECT 
    @ApplicationId, -- 👈 Pass the parameter here
    c.CardHolderName,
    @IsInternationalEnabled, 
    c.CardTypeID,            
    @OldCardId
FROM [Banking].[Cards] c
WHERE c.CardID = @OldCardId";

        public const string InsertRenewApplication = @"
INSERT INTO [Apps].[CardApplications] (
    [CardHolderName], 
    [IsInternationalEnabled], 
    [CardTypeID], 
    [OldCard]
) 
OUTPUT INSERTED.ApplicationID 
SELECT 
    c.CardHolderName,
    @IsInternationalEnabled, -- Or inherit from old card: c.IsInternationalEnabled
    c.CardTypeID,            -- Directly inherits the exact card type!
    @OldCard
FROM [Banking].[Cards] c
WHERE c.CardID = @OldCard";
    };


}
