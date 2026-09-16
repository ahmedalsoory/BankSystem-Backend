using DTOs.AccountOpeningDetail.interfaces;
using DTOs.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningDetail
{
    public class AccountOpeningDetailRequest : IAccountOpeningDetailValidtionDTO , IValidatableDto
    {
        // The Application this data belongs to
        public int ApplicationID { get; set; }

        // The name of the field (e.g., "NationalID", "AddressLine1", "EmploymentStatus")
        public string RequirementKey { get; set; } = string.Empty;

        // The actual value (using string covers most cases, 
        // you can serialize JSON here if needed)
        public string RequirementValue { get; set; } = string.Empty;
    }
}
