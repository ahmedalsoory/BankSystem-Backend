using DTOs.interfaces;
using Shared.Enums.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Transaction
{
    public class DepositRequest : TransactionBaseRequest,IValidatableDto
    {
        public enTransactionType Type { get; } = enTransactionType.Deposit;
        public int ToAccountId { get; set; }
        [JsonConverter(typeof(SmartRowVersionConverter))]
        public byte[] RowVersion { get; set; } // Needed for Optimistic Concurrency
    }
}
