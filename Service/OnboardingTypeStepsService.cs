using DTOs.OnboardingTypeSteps;
using Microsoft.Extensions.Logging;
using RepositoryContracts.OnboardingTypeSteps;
using ServiceContract.IOnboardingTypeSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OnboardingTypeStepsService : BaseService<OnboardingTypeStepsService>
        , IOnboardingTypeStepsReadService
    {
        private readonly IOnboardingTypeStepsReadRepository _read;
        public OnboardingTypeStepsService(IOnboardingTypeStepsReadRepository read,
            ILogger<OnboardingTypeStepsService> logger) : base(logger)
        {
            _read = read;
        }

        public async Task<IEnumerable<OnboardingTypeStepsResponse>> GetAll()
        {

            return await _read.GetAll(); 
        }
    }
}
