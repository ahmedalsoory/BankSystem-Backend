using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class AccountOpeningApplicationQ
    {
        public const string AddApplication = @"
                INSERT INTO Apps.AccountOpeningApplications 
               (ClientID, OnboardingTypeID, Status, CreatedByUserID, CreatedDate, AccountType)
                OUTPUT INSERTED.ApplicationID 
                VALUES (@ClientID, @OnboardingTypeID, 1, @CreatedByUserID, GETDATE(), @AccountType);";

        public const string GetByID = @"SELECT ApplicationID, ClientID, OnboardingTypeID, Status, CreatedDate, CreatedByUserID, Notes, AccountType, Currency, InitialDeposit
                                    FROM     Apps.AccountOpeningApplications
                                    WHERE ApplicationID = @ApplicationID";
        public const string UpdateStatus = @"
              UPDATE Apps.AccountOpeningApplications
              SET Status = @Status
              WHERE ApplicationID = @ApplicationID";
    }
}

