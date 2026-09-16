using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningWorkflow.interfaces
{
    public interface IWorkflowCompletedValidationDTO
    {
        int ApplicationID { get; }
        string StepName { get; }
    }
}
