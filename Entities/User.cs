using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }

        // The unique handle used for login
        public string Username { get; set; } = string.Empty;

        // This stores the 'Slow Hash' (e.g., BCrypt or Argon2) for DB-level verification
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        public bool IsActive { get; set; } = true;

        // Foreign Key to Person
        public int PersonId { get; set; }
        public virtual Person Person { get; set; } = null!;
        public virtual Client? Client { get; set; }
    }
}
