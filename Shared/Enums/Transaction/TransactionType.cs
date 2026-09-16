using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.Transaction
{
    public enum TransactionType : byte
    {
        Deposit = 1,
        Withdrawal = 2,
        Transfer = 3
    }
    public static class TransactionMappingExtensions
    {
        public static string ToDescription(this TransactionType type) => type switch
        {
            TransactionType.Deposit => "Deposit",
            TransactionType.Withdrawal => "Withdrawal",
            TransactionType.Transfer => "Transfer",
            _ => "Unknown"
        };
    }
}
