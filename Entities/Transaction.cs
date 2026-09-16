using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{

        public class Transaction
        {
            public int Id { get; set; }

            // Audit Reference (e.g., TXN-99283-AX)
            public string Reference { get; set; } = string.Empty;

            public decimal Amount { get; set; }
            public TransactionType Type { get; set; }
            public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

            // --- The Money Flow ---

            // Nullable because a 'Deposit' has no Source Account
            public int? FromAccountId { get; set; }
            public virtual Account? FromAccount { get; set; }

            // Nullable because a 'Withdrawal' has no Destination Account
            public int? ToAccountId { get; set; }
            public virtual Account? ToAccount { get; set; }

            // The Admin or User who executed this (Optional Audit)
            public int ExecutedByUserId { get; set; }
        }

        public enum TransactionType
        {
            Deposit = 1,
        Withdraw = 2,
            Transfer = 3,
            Fee = 4
        }
    }

