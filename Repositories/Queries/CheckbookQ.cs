using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class CheckbookQ
    {
        public const string Insert = @"INSERT INTO Banking.Checkbooks (AccountID, ApplicationID, BeginCheckNumber, EndCheckNumber, Status, CreatedDate)
        OUTPUT INSERTED.CheckbookID
        VALUES (@AccountID, @ApplicationID, @BeginCheckNumber, @EndCheckNumber, @Status, GETDATE());";

        public const string getLastCheck = @"SELECT ISNULL(MAX(EndCheckNumber), 100000) 
            FROM Banking.Checkbooks 
            WHERE AccountID = @AccountID";

        public const string GetDetailsForIssueFirstTime = @"
SELECT Apps.AccountApplications.AccountID, Apps.CheckbookApplications.NumberOfLeaves
FROM     Apps.AccountApplications INNER JOIN
                  Apps.CheckbookApplications ON Apps.AccountApplications.ApplicationID = Apps.CheckbookApplications.ApplicationID
				  where CheckbookApplications.ApplicationID = @ApplicationID";

        public const string GetDetailsForReplacementOrRenew = @"SELECT Banking.Checkbooks.AccountID, Banking.Checkbooks.BeginCheckNumber, Apps.CheckbookApplications.NumberOfLeaves
FROM     Banking.Checkbooks INNER JOIN
                  Apps.CheckbookApplications ON Banking.Checkbooks.CheckbookID = Apps.CheckbookApplications.OldCheckbookID
				  where CheckbookApplications.ApplicationID = @ApplicationID ";

        public const string UpdateStatus = @"
        UPDATE Banking.Checkbooks
        SET Status = @Status
        WHERE CheckbookID = @CheckbookID;";

    }
}
