using DTOs.interfaces;
using Shared.Enums.Transaction;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Transaction
{
    public class TransactionPagedRequest : IValidatableDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 35;
        public SortedBy_Transaction SortBy { get; set; } = SortedBy_Transaction.TransactionDate;
        public Direction Direction { get; set; } = Direction.DESC;
        public Filter_Transaction? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? CachedTotalCount { get; set; } = null;
    }
}
