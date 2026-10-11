using DTOs.Person;
using System;

namespace Tests.TestBuilders
{
    public enum PersonFailureType
    {
        None,
        PhoneAlreadyExists,
        EmailAlreadyExists,
        InvalidNationalId
    }

    public class PersonTestScenario
    {
        public PersonFailureType FailureType { get; private set; } = PersonFailureType.None;

        public PersonTestScenario Success()
        {
            FailureType = PersonFailureType.None;
            return this;
        }

        public PersonTestScenario Fail(PersonFailureType failureType)
        {
            FailureType = failureType;
            return this;
        }
    }

    public class PersonBuilder
    {
        protected readonly PersonAddRequest _request = new();
        protected readonly PersonUpdateRequest _updateRequest = new();
        protected PersonTestScenario? _scenario;

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

            _updateRequest.FirstName = firstName;
            _updateRequest.LastName = lastName;
            return this;
        }

        public PersonBuilder WithNationalId(string nationalId)
        {
            _request.NationalId = nationalId;
            _updateRequest.NationalId = nationalId;
            return this;
        }

        public PersonBuilder WithPhone(string phone)
        {
            _request.Phone = phone;
            _updateRequest.Phone = phone;
            return this;
        }

        public PersonBuilder WithEmail(string email)
        {
            _request.Email = email;
            _updateRequest.Email = email;
            return this;
        }

        // Scenario hook for negative testing (e.g., .Create(s => s.Fail(PersonFailureType.EmailAlreadyExists)))
        public PersonBuilder Create(Action<PersonTestScenario> scenarioAction)
        {
            _scenario = new PersonTestScenario();
            scenarioAction(_scenario);
            if(_scenario.FailureType != PersonFailureType.None)
            {
                var dbPerson = PersonDataHelper.GetPersonDataAsync().GetAwaiter().GetResult();
                switch (_scenario.FailureType)
                 {
                     case PersonFailureType.PhoneAlreadyExists:
                         _request.Phone = dbPerson.Phone;
                         break;

                     case PersonFailureType.EmailAlreadyExists:
                        _request.Email = dbPerson.Email;
                         break;

                     case PersonFailureType.InvalidNationalId:
                         _request.NationalId = dbPerson.NationalId;
                         break;

                 }
            }
            return this;
        }

        // Internal exposures for extension methods and downstream builders
        internal PersonUpdateRequest GetUpdateRequest() => _updateRequest;
        internal PersonAddRequest GetRequest() => _request;
        internal PersonTestScenario? GetScenario() => _scenario;
    }
}