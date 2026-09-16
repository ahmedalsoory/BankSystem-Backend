using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.Checkbook
{
    public enum CheckbookStatus : byte
    {
        PendingApproval = 1,
        Active = 2,
        Exhausted = 3, // All leaves used up
        Cancelled = 4  // Lost, stolen, or closed
    }
}
