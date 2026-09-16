using DTOs.interfaces;
using Shared.Enums.Client;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Client
{
    public class ClientPagedRequest : IValidatableDto
    {
        public int PageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public SortedBy_Client SortBy { get; set; } = SortedBy_Client.Name;
        public Direction Direction { get; set; } = Direction.ACS;
        public Filter_Client? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public int? CachedTotalCount { get; set; } = null;
    }
}
