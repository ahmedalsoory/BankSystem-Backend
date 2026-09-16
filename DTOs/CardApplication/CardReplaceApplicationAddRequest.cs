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
    public class CardReplaceApplicationAddRequest : CardApplicationAddRequest,
       ICardApplicationValidation
    {


        [JsonIgnore]
        public new byte CardTypeID => 0;

        public override byte ApplicationTypeID => (byte)ApplicationType.ReplaceVisaCard;

        public new int? oldCardId {  get; set; }
    }
}
