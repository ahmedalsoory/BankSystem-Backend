using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Account
{
    public class AccountResponse
    {
        public int AccountID { get; set; }
        public int ClientID { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public byte AccountType { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserID { get; set; }
        public byte[] RowVersion { get; set; }

    }
}

