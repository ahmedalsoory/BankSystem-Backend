using DTOs.interfaces;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CardApplication
{
    public class InternationalCardAddRequest : CardApplicationAddRequest, IValidatableDto
    {
        public InternationalCardAddRequest() {

            this.ApplicationTypeID = (byte)ApplicationType.IssueInternationalVisa;
            this.IsInternationalEnabled = true;
          }
       
    }
}
