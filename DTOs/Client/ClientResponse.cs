using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Client
{
    public class ClientResponse
    {

        public int PersonID {  get; init; }
        public  string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string ClientNumber { get; init; } = string.Empty;
        public bool IsActive { get; init; } 
        public byte[] VersionStamp { get; set; } = new byte[0];
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public ClientResponse() { }
        public ClientResponse(int personID,string firstName, string lastName, string email, bool isActive,
            string clientNumber,DateTime createdAt, byte[] versionStamp)
        {
            PersonID = personID;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            IsActive = isActive;
            CreatedAt = createdAt;
            VersionStamp = versionStamp;
            ClientNumber = clientNumber;    
        }
    }
}
