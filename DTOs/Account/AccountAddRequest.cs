using DTOs.Account.interfaces;
using DTOs.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Account
{
    public class AccountAddRequest : IAccountValidtionDTO, IValidatableDto
    {
        public int ClientID { get; set; }
        public byte AccountType { get; set; }
        public string Currency { get; set; } = "USD";
        public int CreatedByUserID { get; set; } = 1;
        public string? AccountNumber {  get; set; }
        public int ApplicationID {  get; set; }
    }
}
