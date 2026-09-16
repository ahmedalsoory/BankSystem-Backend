using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CardType
{
    public class CardTypeAddRequest
    {
        public string? TypeName { get; set; }
        public decimal DefaultWithdrawalLimit {  get; set; }
        public decimal MaxWithdrawalLimit { get; set; }
        public bool RequiresManagerApproval {  get; set; }=false;
        public bool IsActive { get; set; }=true;

    }
}
