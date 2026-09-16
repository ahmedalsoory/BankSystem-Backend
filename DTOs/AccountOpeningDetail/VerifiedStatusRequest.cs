using DTOs.AccountOpeningWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningDetail
{
    public class VerifiedStatusRequest
    {
        public int DetailID {  get; set; }
        public WorkflowCompleteStatusUpdateRequest WorkFlowData { get; set; }
    }
}
