using DTOs;
using DTOs.OnboardingApplicationTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.OnboardingApplicationTypes;

namespace API.Controllers
{

    public class OnboardingApplicationTypesController : MyControllerBase
    {
        private readonly IOnboardingApplicationTypesReadServcie _read;
        public OnboardingApplicationTypesController(IOnboardingApplicationTypesReadServcie read,
            ILoggerFactory loggerFactory) : base(loggerFactory) 
        { 
            _read = read;
        }


        [HttpGet()]

        public async Task<PagedResult<OnboardingApplicationTypesListItem>> GetAll()
        {
            var result = await _read.GetAll().ConfigureAwait(false); 
            return new PagedResult<OnboardingApplicationTypesListItem>()
            {
                Data = result,
                TotalCount = result.Count()

            };
        }
    }
}
