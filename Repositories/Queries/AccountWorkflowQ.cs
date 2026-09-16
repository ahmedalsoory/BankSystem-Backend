using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class AccountWorkflowQ
    {
        public const string AddWorkflowSteps = @"
                INSERT INTO Apps.ApplicationWorkflowSteps (ApplicationID, StepName, IsCompleted)
                SELECT @ApplicationID, StepName, 0 
                FROM Apps.OnboardingTypeSteps 
                WHERE OnboardingTypeID = @OnboardingTypeID
                AND NOT EXISTS (SELECT 1 FROM Apps.ApplicationWorkflowSteps WHERE ApplicationID = @ApplicationID)
                ORDER BY OrderIndex ASC";
        //EmployeeID
        public const string CompleteStep = @"
                UPDATE Apps.ApplicationWorkflowSteps
                SET IsCompleted = 1, CompletedDate = GETDATE(), CompletedByEmployeeID = @EmployeeID
                WHERE ApplicationID = @ApplicationID AND StepName = @StepName AND IsCompleted = 0;";

        public const string CheckIncompleteSteps = @"
                SELECT COUNT(1) FROM Apps.ApplicationWorkflowSteps
                WHERE ApplicationID = @ApplicationID AND IsCompleted = 0";
    }
}

