using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class AccountOpeningDetailQ
    {
        public const string Add = @"
                INSERT INTO Apps.AccountOpeningDetails (ApplicationID, RequirementKey, RequirementValue)
                OUTPUT INSERTED.DetailID 
                VALUES (@ApplicationID, @RequirementKey, @RequirementValue);";

        public const string GetByApplicationId = @"
                SELECT ID, ApplicationID, RequirementKey, RequirementValue, Status 
                FROM Apps.AccountOpeningDetails 
                WHERE ApplicationID = @ApplicationID";

        public const string Update = @"
                UPDATE Apps.AccountOpeningDetails 
                SET Value = @Value, Status = @Status 
                WHERE ApplicationID = @ApplicationID AND RequirementKey = @RequirementKey";

        public const string UpdateStatusAndReason = @"
                UPDATE Apps.AccountOpeningDetails 
                SET Status = @Status , RejectionReason = ISNULL(@RejectionReason, RejectionReason)
                 WHERE DetailID = @DetailID
        ";

        public const string UpdateStatus = @"
                UPDATE Apps.AccountOpeningDetails
SET Status = @NewStatus,
    RejectionReason = @RejectionReason,
    LastModifiedDate = GETDATE()
WHERE DetailId = @DetailId
  AND Status IN (0, 1) 
  AND @NewStatus IN (2, 3)";
    }
}
