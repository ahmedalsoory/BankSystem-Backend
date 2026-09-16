using DTOs.CardApplication.interfaces;
using DTOs.interfaces;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CardApplication
{
    public class CardRenewApplicationAddRequest : CardApplicationAddRequest,
        ICardApplicationValidation
    {
        public CardRenewApplicationAddRequest()=> this.ApplicationTypeID = (byte)ApplicationType.RenewVisaCard;
        
        public new int? oldCardId {  get; set; }
    }
}
