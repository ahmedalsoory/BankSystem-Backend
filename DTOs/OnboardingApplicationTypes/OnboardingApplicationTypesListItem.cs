using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OnboardingApplicationTypes
{
    public class OnboardingApplicationTypesListItem
    {
        public byte OnboardingTypeID { get; set; }
        public string TypeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
