using DTOs.interfaces;
using Shared.Enums.Account;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Account
{
    public class AccountPagedRequest : IValidatableDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public SortedBy_Account SortBy { get; set; } = SortedBy_Account.CreatedDate;
        public Direction Direction { get; set; } = Direction.DESC;
        public Filter_Account? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public int? CachedTotalCount { get; set; } = null;
    }
}
