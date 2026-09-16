using DTOs;
using DTOs.OnboardingTypeSteps;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.IOnboardingTypeSteps;

namespace API.Controllers
{

    public class OnboardingTypeStepsController : MyControllerBase
    {
        private readonly IOnboardingTypeStepsReadService _read;
        public OnboardingTypeStepsController(ILoggerFactory loggerFactory,
            IOnboardingTypeStepsReadService read)
        :base(loggerFactory) 
        {
            _read = read;
        }

        [HttpGet()]
        public async Task<PagedResult<OnboardingTypeStepsResponse>> GetAll()
        { 
            var result =  await _read.GetAll().ConfigureAwait(false);
            return new PagedResult<OnboardingTypeStepsResponse>
            {
                Data = result,
                TotalCount = result.Count()
            };
        }
    }
}
