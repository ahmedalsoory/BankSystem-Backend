using DTOs.interfaces;
using Shared.Enums.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Transaction
{
    public class TransactionBaseRequest: IValidatableDto
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int ? SourceId { get; set; }
        public SourceType_Transaction ? sourceType { get; set; }
    }
}
