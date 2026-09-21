using DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestBuilders
{
    public class PersonBuilder
    {
        protected readonly PersonAddRequest _request = new();

        public PersonBuilder()
        {
            // Safe, unique default values to prevent test collisions
            _request.FirstName = "TestFirst";
            _request.LastName = "TestLast";
            _request.NationalId = "NAT-" + Guid.NewGuid().ToString()[..8];
            _request.Email = $"test_{Guid.NewGuid().ToString()[..6]}@bank.com";
            _request.Phone = "091" + new Random().Next(1000000, 9999999);
            _request.BirthDate = new DateTime(1995, 1, 1);
            _request.Gendor = 'M';
        }

        public PersonBuilder WithName(string firstName, string lastName)
        {
            _request.FirstName = firstName;
            _request.LastName = lastName;
            return this;
        }

        public PersonBuilder WithNationalId(string nationalId)
        {
            _request.NationalId = nationalId;
            return this;
        }
        public PersonBuilder WithPhone(string phone)
        {
            _request.Phone = phone;
            return this;
        }

        
        public PersonBuilder WithEmail(string email)
        {
            _request.Email = email;
            return this;
        }

        // Expose the base request to the extension method
        internal PersonAddRequest GetRequest() => _request;
    }
}
