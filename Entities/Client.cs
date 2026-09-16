using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Client
    {
        public int Id { get; set; }

        // Unique identifier like 'ACC-100293'
        public string ClientNumber { get; set; } = string.Empty;

        // Used for your logic to prioritize or limit certain clients
        public byte RiskLevel { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key to Person
        public int PersonId { get; set; }
        public virtual Person Person { get; set; } = null!;
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        // Link to many Accounts (Savings, Current, etc.)
        public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
    }
}
