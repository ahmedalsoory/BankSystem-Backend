using DTOs.OnboardingApplicationTypes;
using Microsoft.Extensions.Logging;
using RepositoryContracts.OnboardingApplicationTypes;
using ServiceContract.OnboardingApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OnboardingApplicationTypesServcie :BaseService<OnboardingApplicationTypesServcie>,
        IOnboardingApplicationTypesReadServcie
    {

        private readonly IOnboardingApplicationTypesReadRepository _read;
        public OnboardingApplicationTypesServcie(IOnboardingApplicationTypesReadRepository read,
            ILogger<OnboardingApplicationTypesServcie> logger) : base(logger) 
        {
            _read = read;
        }


        public async Task<IEnumerable<OnboardingApplicationTypesListItem>> GetAll()
        {
            return await _read.GetAll();
        }


    }
}
