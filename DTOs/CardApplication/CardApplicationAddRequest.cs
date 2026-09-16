using DTOs.AccountApplications;
using DTOs.CardApplication.interfaces;
using DTOs.interfaces;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.CardApplication
{
    public class CardApplicationAddRequest  : AccountApplicationAddRequest
        ,ICardApplicationValidation
    {

        public override byte ApplicationTypeID => (byte)ApplicationType.IssueLocalVisa;
        public  byte CardTypeID { get; set; }

        public bool IsInternationalEnabled { get; protected set; } =false;
        [JsonIgnore]
        public virtual int? oldCardId { get; set; } = null;
    }
}
