using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningDetail
{
    public class AccountOpeningDetailListItem
    {
        // Matches columns from [Apps].[AccountOpeningDetails]
        public int DetailID { get; set; }
        public int ApplicationID { get; set; }
        public string? RequirementKey { get; set; }
        public string? RequirementValue { get; set; }
        public byte Status { get; set; } // Matches tinyint
        public string? RejectionReason { get; set; }
    }
}
