using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class OnboardingApplicationTypesQ
    {
        public static string GetAll = @"SELECT OnboardingTypeID , TypeName , Description 
                                    , IsActive FROM Apps.OnboardingApplicationTypes";
    }
}
