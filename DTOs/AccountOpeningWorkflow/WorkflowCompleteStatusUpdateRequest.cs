using DTOs.AccountOpeningWorkflow.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningWorkflow
{
    public class WorkflowCompleteStatusUpdateRequest : IWorkflowValidationDTO
    {
        public int ApplicationID { get; set; }
        public string StepName { get; set; } = string.Empty;
        public int EmployeeID { get; set; } =  1;
        public string? Notes { get; set; } // Added for audit/reasoning
    }
}
