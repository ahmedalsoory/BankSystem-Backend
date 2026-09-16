using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Person
{
    public class PersonResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserID {  get; set; }
        public byte[] RowVersion { get; set; } = new byte[0];
        public string? ImagePath { get; set; } = string.Empty;
        public char Gendor { get; set; }
    }
}
