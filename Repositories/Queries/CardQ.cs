using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class CardQ
    {
        public static  string Insert = @"INSERT INTO [Banking].[Cards] (
            [AccountID], [ApplicationID], [CardTypeID], [CardNumberHash], 
            [MaskedCardNumber], [CardHolderName], [ExpirationDate], [CVVHash], 
            [IsInternationalEnabled], [Status], [DailyWithdrawalLimit], 
            [DailyOnlinePurchaseLimit], [CreatedDate], [PinHash]
        )
        OUTPUT INSERTED.[CardID]
        VALUES (
            @AccountID, @ApplicationID, @CardTypeID, @CardNumberHash, 
            @MaskedCardNumber, @CardHolderName, @ExpirationDate, @CVVHash, 
            @IsInternationalEnabled, @Status, @DailyWithdrawalLimit, 
            @DailyOnlinePurchaseLimit, GETDATE(), @PinHash
        );";
        public static string GetDetailsForReplacementOrRenew = @"select CardTypeID , AccountID , CardHolderName from Banking.Cards where ApplicationID = @ApplicationID";

        public static string GetDetailsForIssueFirstTime = @"SELECT Banking.Accounts.AccountID,Core.Persons.FirstName + ' ' + Core.Persons.LastName AS CardHolderName, Apps.CardApplications.CardTypeID
FROM     Core.Clients INNER JOIN
                  Banking.Accounts ON Core.Clients.PersonId = Banking.Accounts.ClientID INNER JOIN
                  Core.Persons ON Core.Clients.PersonId = Core.Persons.Id AND Core.Clients.PersonId = Core.Persons.Id AND Core.Clients.PersonId = Core.Persons.Id INNER JOIN
                  Apps.AccountApplications ON Banking.Accounts.AccountID = Apps.AccountApplications.AccountID AND Banking.Accounts.AccountID = Apps.AccountApplications.AccountID INNER JOIN
                  Apps.CardApplications ON Apps.AccountApplications.ApplicationID = Apps.CardApplications.ApplicationID AND Apps.AccountApplications.ApplicationID = Apps.CardApplications.ApplicationID AND 
                  Apps.AccountApplications.ApplicationID = Apps.CardApplications.ApplicationID

				  where Apps.CardApplications.ApplicationID= @ApplicationID";

        public static string UpdateStatus = @"UPDATE Banking.Cards SET Status = @newStatus
WHERE CardID = @cardId";

        public static string GetCardIdByApplicationId = @"   SELECT CardID 
        FROM Banking.Cards 
        WHERE ApplicationID = @ApplicationID";
    }
}
