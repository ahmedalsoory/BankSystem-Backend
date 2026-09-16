using DTOs;
using DTOs.CardType;
using DTOs.OnboardingTypeSteps;
using Microsoft.Extensions.Logging;
using RepositoryContracts.CardType;
using RepositoryContracts.OnboardingTypeSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CardTypeServices: BaseService<CardTypeServices>
    {
        private readonly ICardTypeReadRepository _read;
        public CardTypeServices(ICardTypeReadRepository read,
            ILogger<CardTypeServices> logger) : base(logger)
        {
            _read = read;
        }

        public async Task<PagedResult<CardTypeResponse>> GetAll()
        {

            var result =  await _read.GetAll();
            return new PagedResult<CardTypeResponse>()
            {
                Data = result,
                TotalCount = result.Count()
            };
        }

    }
}
