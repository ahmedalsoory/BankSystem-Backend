using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.AccountApplications
{
    public enum ApplicationStatus : byte
    {
        New = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }
}
