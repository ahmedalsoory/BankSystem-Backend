using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.CheckLeaf
{
    public enum CheckLeafStatus : byte
    {
        Active = 1,      // Ready to be used
        Used = 2,        // Already processed / cashed
        Disabled = 3,    // Cancelled or disabled (e.g., due to replacement)
        Stolen = 4       // Explicitly reported stolen/lost
    }
}
