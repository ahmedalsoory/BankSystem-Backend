using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningApplication
{
    public class AccountOpeningApplicationResponse
    {
        public int ApplicationID { get; set; }
        public int ClientID { get; set; }

        // Links to the template (Student, Corporate, etc.)
        public byte Status { get; set; }
        public byte OnboardingTypeID { get; set; }

        // The ID of the employee or system user creating the app
        public int CreatedByUserID { get; set; }

        // Optional notes or remarks regarding the application
        public string? Notes { get; set; }

        public byte AccountType { get; set; }

        // Include any other metadata that should be saved at the moment of creation
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? Currency { get; set; }

        public decimal InitialDeposit { get; set; }

    }
}
