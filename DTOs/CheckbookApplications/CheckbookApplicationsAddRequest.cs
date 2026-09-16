using DTOs.AccountApplications;
using DTOs.CheckbookApplications.interfaces;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.CheckbookApplications
{
    public class CheckbookApplicationsAddRequest : AccountApplicationAddRequest, ICheckbookApplicationValidation
    {
        public CheckbookApplicationsAddRequest() => this.ApplicationTypeID = (byte)ApplicationType.IssueCheckbook; // Adjust enum name as needed

        public byte NumberOfLeaves { get; set; } = 25;
        public bool IsUrgentProcessing { get; set; } = false;
        public byte DeliveryMethod { get; set; } = 1;

        [JsonIgnore]
        public virtual int? OldCheckbookID { get; set; } = null;
    }
}
