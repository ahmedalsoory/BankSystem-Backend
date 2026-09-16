using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string AccountHolder { get; set; } =string.Empty;
        public string Currency { get; set; } = "USD";
        public AccountType Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }
    }

    public enum AccountType { Savings = 1, Current = 2, Fixed = 3 }
}
