using Shared.Enums.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Client
{
    public class ClientDetailDto
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string NationalId { get; set; }= string.Empty;
        public DateTime BirthDate { get; set; }
        public char Gendor { get; set; }
        public string? ImagePath {  get; set; }

        // Client-specific data
        public string ClientNumber { get; set; } = string.Empty;
        public byte RiskLevel { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinDate { get; set; }

        // Concurrency Stamps - Essential for Banking Security
        public byte[] PersonVersion { get; set; }=Array.Empty<byte>();
        public byte[] ClientVersion { get; set; } = Array.Empty<byte>();


        public override bool Equals(object? obj)
        {
            if (obj is not ClientDetailDto other) return false;

            return PersonID == other.PersonID &&
                   FirstName == other.FirstName &&
                   LastName == other.LastName &&
                   Email == other.Email &&
                   Phone == other.Phone &&
                   NationalId == other.NationalId &&
                   BirthDate == other.BirthDate &&
                   Gendor == other.Gendor &&
                   ImagePath == other.ImagePath &&
                   ClientNumber == other.ClientNumber &&
                   RiskLevel == other.RiskLevel &&
                   IsActive == other.IsActive &&
                   JoinDate == other.JoinDate &&
                   // For byte arrays (Concurrency Stamps), we compare the sequences
                   (PersonVersion?.SequenceEqual(other.PersonVersion) ?? true) &&
                   (ClientVersion?.SequenceEqual(other.ClientVersion) ?? true);
        }

        public override int GetHashCode()
        {
            // Essential when overriding Equals to keep the object "findable" in Collections
            var hash = new HashCode();
            hash.Add(PersonID);
            hash.Add(ClientNumber);
            return hash.ToHashCode();
        }
    }
}
