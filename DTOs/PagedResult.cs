using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T>? Data { get; set; }
        public int TotalCount { get; set; }
    }
}
