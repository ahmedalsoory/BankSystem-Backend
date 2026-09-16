using DTOs.OnboardingTypeSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.OnboardingTypeSteps
{
    public interface IOnboardingTypeStepsReadRepository
    {
        Task<IEnumerable<OnboardingTypeStepsResponse>> GetAll();
    }
}
