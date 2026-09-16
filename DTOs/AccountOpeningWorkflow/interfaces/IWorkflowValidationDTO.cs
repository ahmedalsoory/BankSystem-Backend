using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningWorkflow.interfaces
{
    public interface IWorkflowValidationDTO
    {
        public int ApplicationID { get; set; }
        public string StepName { get; set; }
    }
}
