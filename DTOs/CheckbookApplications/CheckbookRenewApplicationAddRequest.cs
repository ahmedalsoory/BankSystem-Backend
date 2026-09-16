using DTOs.CheckbookApplications.interfaces;
using DTOs.interfaces;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CheckbookApplications
{
    public class CheckbookRenewApplicationAddRequest : CheckbookApplicationsAddRequest, 
        ICheckbookApplicationValidation,
        IValidatableDto
    {
        public CheckbookRenewApplicationAddRequest() => this.ApplicationTypeID = (byte)ApplicationType.RenewCheckbook; // Adjust enum name as needed

        public new int? OldCheckbookID { get; set; }
    }
}
