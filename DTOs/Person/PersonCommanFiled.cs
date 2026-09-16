using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.interfaces;
using DTOs.Person.interfaces;

namespace DTOs.Person
{
    public  class PersonCommanFiled : IPersonSchema,IValidatableDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? ImagePath { get; set; } = string.Empty;
        public char Gendor { get; set; }
    }
}
