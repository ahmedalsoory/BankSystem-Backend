using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Transaction.map
{
    public static class TransactionMappingExtensions
    {
        public static string ToDescription(this byte type) => type switch
        {
            1 => "Deposit",
            2 => "Withdrawal",
            3 => "Transfer",
            _ => "Unknown"
        };
    }
}
