using DTOs.Transaction.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Transaction
{
    public class ClientTransactionResponse
    {
        public decimal Amount { get; set; }
        public byte Type { get; set; }
        public string TypeDescription => this.Type.ToDescription();
        public DateTime TransactionDate { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
    }
}
