using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class CheckbookApplicationQ
    {
        public const string InsertNewCheckbookApplication = @"
            INSERT INTO [Apps].[CheckbookApplications] (
                [ApplicationID],
                [NumberOfLeaves],
                [IsUrgentProcessing],
                [DeliveryMethod],
                [OldCheckbookID]
            ) 
            OUTPUT INSERTED.ApplicationID
            VALUES (
                @ApplicationId,
                @NumberOfLeaves,
                @IsUrgentProcessing,
                @DeliveryMethod,
                NULL
            )";

        public const string InsertRenewApplication = @"
            INSERT INTO [Apps].[CheckbookApplications] (
                [ApplicationID],
                [NumberOfLeaves],
                [IsUrgentProcessing],
                [DeliveryMethod],
                [OldCheckbookID]
            ) 
            OUTPUT INSERTED.ApplicationID
            VALUES (
                @ApplicationId,
                @NumberOfLeaves,
                @IsUrgentProcessing,
                @DeliveryMethod,
                @OldCheckbookID
            )";

        public const string InsertReplaceApplication = @"
            INSERT INTO [Apps].[CheckbookApplications] (
                [ApplicationID],
                [NumberOfLeaves],
                [IsUrgentProcessing],
                [DeliveryMethod],
                [OldCheckbookID]
            ) 
            OUTPUT INSERTED.ApplicationID
            VALUES (
                @ApplicationId,
                @NumberOfLeaves,
                @IsUrgentProcessing,
                @DeliveryMethod,
                @OldCheckbookID
            )";
    }
}