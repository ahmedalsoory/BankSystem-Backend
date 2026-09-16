using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class OnboardingTypeStepsQ
    {
        public static string GetAll= @"
            select 
            MappingID , OnboardingTypeID,StepName , OrderIndex
            from Apps.OnboardingTypeSteps
        ";
    }
}
