using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountProducts
{
    public class AccountProductsResponse
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public decimal MinOpeningDeposit { get; set; }
        public decimal ApplicationFee { get; set; }
        public decimal InterestRate {  get; set; }
    }
}
