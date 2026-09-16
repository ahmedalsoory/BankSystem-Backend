using DTOs.OnboardingApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.OnboardingApplicationTypes
{
    public interface IOnboardingApplicationTypesReadServcie
    {
        Task<IEnumerable<OnboardingApplicationTypesListItem>> GetAll();
    }
}
