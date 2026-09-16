using DTOs.OnboardingApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.OnboardingApplicationTypes
{
    public interface IOnboardingApplicationTypesReadRepository
    {
        Task<IEnumerable<OnboardingApplicationTypesListItem>> GetAll();
    }
}
