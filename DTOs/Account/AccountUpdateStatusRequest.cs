using DTOs.interfaces;
using Shared.Enums.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Account
{
    public class AccountUpdateStatusRequest : IVersioned
    {
        public int AccountID { get; set; }
        public enAccountStatus Status { get; set; }

        [JsonConverter(typeof(SmartRowVersionConverter))]
        public byte[] RowVersion { get; set; }
    }
}
