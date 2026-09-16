using Shared.Enums.AccountOpeningApplications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.interfaces;

namespace DTOs.AccountOpeningApplication
{
    public class AccountOpeningPagedRequest : IValidatableDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public SortedBy_AccountOpening SortBy { get; set; } = SortedBy_AccountOpening.CreatedDate;
        public Direction Direction { get; set; } = Direction.DESC;

        public Filter_AccountOpening? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public int? CachedTotalCount { get; set; } = null;
    }
}
