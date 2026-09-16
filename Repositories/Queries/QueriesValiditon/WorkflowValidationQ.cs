using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class WorkflowValidationQ
    {
        public const string GetStepOrderIndex = @"
        SELECT ts.OrderIndex 
        FROM Apps.OnboardingTypeSteps ts
        JOIN Apps.AccountOpeningApplications app ON ts.OnboardingTypeID = app.OnboardingTypeID
        WHERE app.ApplicationID = @ApplicationID 
          AND ts.StepName = @StepName;";

        public const string CheckForBlockingSteps = @"
        SELECT COUNT(1) 
        FROM Apps.ApplicationWorkflowSteps s WITH (UPDLOCK, ROWLOCK)
        JOIN Apps.OnboardingTypeSteps ts ON s.StepName = ts.StepName 
            AND ts.OnboardingTypeID = (SELECT OnboardingTypeID FROM Apps.AccountOpeningApplications WHERE ApplicationID = @ApplicationID)
        WHERE s.ApplicationID = @ApplicationID 
          AND s.IsCompleted = 0 
          AND ts.OrderIndex < @TargetIndex;";
    }
}
