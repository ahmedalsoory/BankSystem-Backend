using DTOs.AccountOpeningWorkflow.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningWorkflow
{
    public class WorkflowInitializationRequest 
    {
        public int ApplicationID { get; set; }
        public byte OnboardingTypeID { get; set; }

    }
}
