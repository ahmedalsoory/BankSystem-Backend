using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningDetail
{
    public class AccountOpeningDetailResponse
    {
        // Unique ID for this specific detail record
        public int DetailID { get; set; }

        // The Application this data belongs to
        public int ApplicationID { get; set; }

        // The name of the field (e.g., "NationalID")
        public string RequirementKey { get; set; } = string.Empty;

        // The stored value
        public string RequirementValue { get; set; } = string.Empty;
    }
}
