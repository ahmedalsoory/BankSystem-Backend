using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.Account
{
    public enum enAccountStatus : byte
    {
        PendingApproval = 1,
        Active = 2,
        Frozen = 3,
        Suspended = 4,
        Closed = 5
    }
}
