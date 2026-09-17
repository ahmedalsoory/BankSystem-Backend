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
        OUTPUT INSERTED.Id
        VALUES (@AccountID, @ApplicationID, @BeginCheckNumber, @EndCheckNumber, @Status, GETDATE());";

        public const string getLastCheck = @"SELECT ISNULL(MAX(EndCheckNumber), 100000) 
            FROM Banking.Checkbooks 
            WHERE AccountID = @AccountID";
    }
}
