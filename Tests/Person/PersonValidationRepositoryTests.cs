using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Repositories.Validations;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Person
{
    public class PersonValidationRepositoryTests : IDisposable
    {
        private readonly PersonValidationRepository _repo;
        private readonly DbContextScope _dbScope;

        public PersonValidationRepositoryTests()
        {
            // 1. Create a "fake" configuration pointing to your TEST database
            var myConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                {"ConnectionStrings:DefaultConnection",
                       "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BankSystemDB;Max Pool Size=5;" +
                       "Integrated Security=True;Connect Timeout=5;Encrypt=False;Trust Server Certificate=False;" +
                       "Application Intent=ReadWrite;Multi Subnet Failover=False"}
                })
                .Build();

            // 2. Initialize your real scope with the test config
            _dbScope = new DbContextScope(myConfiguration);

            // 3. Inject the scope into your repository
            _repo = new PersonValidationRepository(_dbScope, new Context());
        }

        [Fact]
        public async Task GetPersonConflictReasonsAsync_ShouldDetectDuplicateNationalId()
        {
            // Arrange: Use a unique ID to avoid interference with other tests
            var uniqueId = "TEST-" + Guid.NewGuid().ToString().Substring(0, 5);

            // Act: Check for conflicts (Assuming this ID doesn't exist yet)
            var resultBefore = await _repo.GetConflictReasonsAsync(uniqueId, "test@test.com", "123");

            // Assert: Should be empty initially
            Assert.Empty(resultBefore);
        }

        [Fact]
        public async Task GetPersonConflictReasonsAsync_ShouldReturnDuplicateNationalId()
        {


            // Act: Check for conflicts (Assuming this ID doesn't exist yet)
            var resultBefore = await _repo.GetConflictReasonsAsync("NID-1000001", "test@test.com", "123");

            // Assert: Should be empty initially
            resultBefore.Should().Contain("NationalId");
        }

        public void Dispose()
        {
            _dbScope.Dispose();
        }
    }
}
