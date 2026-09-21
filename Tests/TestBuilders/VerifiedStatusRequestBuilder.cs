using DTOs.AccountOpeningDetail;
using DTOs.AccountOpeningWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestBuilders
{
    public class VerifiedStatusRequestBuilder
    {
        private readonly VerifiedStatusRequest _request = new()
        {
            WorkFlowData = new WorkflowCompleteStatusUpdateRequest()
        };

        public VerifiedStatusRequestBuilder ForDetail(int detailId)
        {
            _request.DetailID = detailId;
            return this;
        }

        public VerifiedStatusRequestBuilder ForApplication(int applicationId)
        {
            _request.WorkFlowData.ApplicationID = applicationId;
            return this;
        }

        public VerifiedStatusRequestBuilder ForStep(string stepName)
        {
            _request.WorkFlowData.StepName = stepName;
            return this;
        }

        public VerifiedStatusRequestBuilder ByEmployee(int employeeId)
        {
            _request.WorkFlowData.EmployeeID = employeeId;
            return this;
        }

        public VerifiedStatusRequestBuilder WithNotes(string? notes)
        {
            _request.WorkFlowData.Notes = notes;
            return this;
        }

        public VerifiedStatusRequest Build()
        {
            return _request;
        }
    }
}
