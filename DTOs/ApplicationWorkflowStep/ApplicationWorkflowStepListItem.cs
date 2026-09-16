using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ApplicationWorkflowStep
{
    public class ApplicationWorkflowStepListItem
    {
        public int stepID {  get; set; }
        public int applicationID { get; set; }
        public string stepName { get; set; }
        public bool isCompleted { get; set; }
        public DateTime completedDate {  get; set; }
        public int completedByEmployeeID { get; set; }
    }
}
