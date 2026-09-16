using DTOs.interfaces;
using Shared.Enums.WorkflowStep;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ApplicationWorkflowStep
{
    public class ApplicationWorkflowStepPagedRequest 
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortedBy_WorkflowStep SortBy { get; set; } = SortedBy_WorkflowStep.StepID;
        public Direction Direction { get; set; } = Direction.DESC;

        // Generic filtering
        public Filter_WorkflowStep? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public int? CachedTotalCount { get; set; } = null;
    }
}
