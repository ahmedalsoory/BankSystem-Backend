using DTOs.interfaces;
using DTOs.Transaction.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Transaction
{
    public class WithdrawRequest : TransactionBaseRequest,ITransactionValidationDTO, IValidatableDto
    {
        public int FromAccountId { get; set; }
        [JsonConverter(typeof(SmartRowVersionConverter))]
        public byte[] RowVersion { get; set; }
    }
}
