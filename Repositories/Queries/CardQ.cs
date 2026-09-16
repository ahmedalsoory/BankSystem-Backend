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
        public static string GetDeatils = @"select CardTypeID , AccountID , CardHolderName from Apps.CardApplications ac
inner join Apps.AccountApplications a on ac.ApplicationID = a.ApplicationID
where a.ApplicationID = @ApplicationID";

        public static string UpdateStatus = @"UPDATE Banking.Cards SET Status = @newStatus
WHERE CardID = @cardId";

        public static string GetCardIdByApplicationId = @"   SELECT CardID 
        FROM Banking.Cards 
        WHERE ApplicationID = @ApplicationID";
    }
}
