using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class AccountQ
    {
        public const string GetById = @"
                SELECT AccountID, ClientID, AccountNumber, AccountType, Balance, Currency, Status, 
                       CreatedDate, RowVersion, CreatedByUserID 
                FROM Banking.Accounts WHERE AccountID = @id";

        public const string GetByNumber = @"
                SELECT AccountID, ClientID, AccountNumber, AccountType, Balance, Currency, Status, 
                       CreatedDate, RowVersion, CreatedByUserID
                FROM Banking.Accounts 
                WHERE AccountNumber = @accountNumber";

        public const string GetByClientId = @"
                SELECT AccountID, ClientID, AccountNumber, AccountType, Balance, Currency, Status 
                FROM Banking.Accounts where ClientID = @clientId";

        public const string Add = @"
               INSERT INTO Banking.Accounts (ClientID, AccountNumber, AccountType, Currency, Status, 
CreatedByUserID, CreatedDate,ApplicationID)
    OUTPUT INSERTED.AccountID
    VALUES (@ClientID, @AccountNumber, @AccountType, @Currency, 1, @CreatedByUserID, GETDATE(),@ApplicationID);";

        public const string UpdateStatus = @"UPDATE Banking.Accounts SET
Status = @Status
WHERE AccountID = @AccountID AND RowVersion=@RowVersion";

        public const string LockAccountForTransfer = @"
            select * from Banking.Accounts WITH (UPDLOCK)
                where AccountID in (@FromAccountId,@ToAccountId)
                order by AccountID
            ";



    }
}
