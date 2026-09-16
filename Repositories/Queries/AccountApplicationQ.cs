using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class AccountApplicationQ
    {
        public const string GetById = @"
            SELECT ApplicationID, AccountID, ApplicationTypeID, Status, 
                   CreatedDate, CreatedByUserID, LastModifiedDate, LastModifiedByUserID, 
                   Notes, RowVersion 
            FROM Apps.AccountApplications 
            WHERE ApplicationID = @id";

        public const string Add = @"
            INSERT INTO Apps.AccountApplications (AccountID, ApplicationTypeID, Status, CreatedByUserID, CreatedDate, Notes)
            OUTPUT INSERTED.ApplicationID 
            VALUES (@AccountID, @ApplicationTypeID, 1, @CreatedByUserID, GETDATE(), @Notes);";

        public const string UpdateStatus = @"
                                UPDATE Apps.AccountApplications
                    SET Status = @NewStatus,
                        LastModifiedDate = GETDATE(),
                        LastModifiedByUserID = @ModifiedByUserId,
                        Notes = CASE WHEN @Notes IS NOT NULL THEN @Notes ELSE Notes END
                    WHERE ApplicationID = @Id 
                      AND Status = 1 AND @NewStatus in (2,3,4) ";

        public const string GetApplicationTypeByIdAsync = @"
                        select ApplicationTypeID from Apps.AccountApplications
                        where ApplicationID = @ApplicationID";
    }
}

