using DTOs.interfaces;
using Shared.Enums.AccountOpeningDetails;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningDetail
{
    public class AccountOpeningDetailPagedRequest : IValidatableDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public SortedBy_AccountOpeningDetail SortBy { get; set; } = SortedBy_AccountOpeningDetail.DetailID;
        public Direction Direction { get; set; } = Direction.DESC;

        public Filter_AccountOpeningDetail? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public int? CachedTotalCount { get; set; } = null;
    }
}
