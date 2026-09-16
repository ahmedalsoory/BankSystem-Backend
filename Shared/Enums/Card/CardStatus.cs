using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.Card
{
    public enum CardStatus : byte
    {
        PendingApproval = 1,
        PendingActivation = 2, // Physical card ordered/shipped, waiting for user to activate
        Active = 3,            // Ready for use
        Frozen = 4,            // Temporarily locked by user (lost/misplaced)
        Cancelled = 5,         // Permanently closed (expired, reported stolen, or replaced)
        Blocked = 6            // Blocked by system due to suspected fraud
    }
}
