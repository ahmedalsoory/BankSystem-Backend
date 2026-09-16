using DTOs.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningApplication
{
    public class AccountOpeningAppListItem
    {
        public int ApplicationID { get; set; }
        public int ClientID { get; set; }

        public byte Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public byte OnboardingTypeID {  get; set; }
        public byte TotalSteps { get; set; }
        public byte CompletedSteps { get; set; }
    }
}
