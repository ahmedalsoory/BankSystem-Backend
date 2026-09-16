using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.AccountOpeningDetails
{
    public enum DetailStatus : byte
    {
        Pending = 0,      // Data entered by user, ready for system check
        UnderReview = 1,  // System is actively verifying the data
        Verified = 2,     // Data passed verification
        Rejected = 3      // Data failed verification
    }
}
