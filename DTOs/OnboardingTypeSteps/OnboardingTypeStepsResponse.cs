using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OnboardingTypeSteps
{
    public class OnboardingTypeStepsResponse
    {
        public byte MappingID { get; set; }
        public byte OnboardingTypeID { get; set; }
        public string StepName { get; set; }
        public byte OrderIndex {  get; set; }
    }
}
