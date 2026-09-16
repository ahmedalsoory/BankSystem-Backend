using Shared.Enums.AccountApplications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.interfaces;

namespace DTOs.AccountApplications
{
    public class AccountApplicationPagedRequest : IValidatableDto
    {
        public int? AccountId { get; set; } = null!;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortedBy_AccountApplication SortBy { get; set; } = SortedBy_AccountApplication.CreatedDate;
        public Direction Direction { get; set; } = Direction.DESC;
        public Filter_AccountApplication? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public int? CachedTotalCount { get; set; } = null;
    }
}
