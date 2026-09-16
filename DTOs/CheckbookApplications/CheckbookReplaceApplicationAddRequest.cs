using DTOs.CheckbookApplications.interfaces;
using DTOs.interfaces;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.CheckbookApplications
{
    public class CheckbookReplaceApplicationAddRequest : CheckbookApplicationsAddRequest, IValidatableDto
    , ICheckbookApplicationValidation
    {
        public CheckbookReplaceApplicationAddRequest() => this.ApplicationTypeID = (byte)ApplicationType.ReplaceCheckbook; // Adjust enum name as needed

        [JsonIgnore]
        public new byte NumberOfLeaves => 25; // Or whatever default lock rule you need for replacement
        public new int? OldCheckbookID { get; set; }
    }
}
