using DTOs.Client;
using Microsoft.Extensions.Configuration;
using Repositories;
using Shared.Enums.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RepositoryContracts.PersonRepo;
using DTOs.Person;
using Shared.Enums;
using Repositories.Core;

namespace Tests.Client
{
    public class ClientRepositoryTests : IDisposable
    {
        private readonly ClientRepository _repo;
        private readonly DbContextScope _dbScope;
        private readonly IOptions<DbSettings> _options;

        public ClientRepositoryTests()
        {
            // 1. Setup Configuration & Options
            var myConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                {"ConnectionStrings:DefaultConnection",ConnectionString.Connection }
                }).Build();

            // Wrap the settings in IOptions (required by the constructor)
            var settings = new DbSettings { DefaultConnection = myConfiguration.GetSection("DbSettings")["DefaultConnection"] };
            _options = Microsoft.Extensions.Options.Options.Create(settings);

            // 2. Initialize Dependencies
            _dbScope = new DbContextScope(myConfiguration);
            var errorContext = new Context();

            // 3. Create a Lazy Person Repo (needed for RegisterClientAsync)
            // We pass a function that returns a real Person Repository
            var PersonRepo =new PersonRepository(_dbScope, errorContext);

            // 4. FIXING THE ERROR: Pass all 4 required parameters
            _repo = new ClientRepository(_dbScope, errorContext, _options,PersonRepo);
        }

        [Fact]
        public async Task RegisterClientAsync_ShouldCommitSuccessfully()
        {
            // Arrange
            var uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 5);

            var request = new ClientAddRequest
            {
                Person = new PersonAddRequest
                {
                    FirstName = "",
                    BirthDate = new DateTime(2000,12,2),
                    LastName = "User",
                    NationalId = "N-" + uniqueSuffix,
                    Phone = "555-" + uniqueSuffix,
                    Email = "test-" + uniqueSuffix + "@bank.com" 
                },
                ClientDetails = new ClientAddRequest_ClientData
                {
                    RiskLevel = 1,
                    IsActive = true
                }
            };

            // Act
            var result = await _repo.RegisterClientAsync(request);

            // Assert
            Assert.True(result);
        }
    

        [Fact]
        public async Task GetByPersonIDAsync_ShouldReturnCorrectJoinedData()
        {
            // Arrange: Use a known ID from your seed data or the one created above
            int knownPersonId = 400069;

            // Act
            var result = await _repo.GetByPersonIDAsync(knownPersonId);

            // Assert
            if (result != null)
            {
                Assert.NotNull(result.ClientNumber);
                Assert.NotNull(result.FirstName);
                Assert.True(result.PersonID == knownPersonId);
            }
        }

        [Fact]
        public async Task GetClientsPagedAsync_ShouldReturnDataAndTotalCount()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;

            // Act
            var result = await _repo.GetClientsPagedAsyncAsList(
                pageNumber,
                pageSize,
                SortedBy_Client.Name,
                Direction.ACS);

            // Assert
            Assert.NotNull(result.Data);
            Assert.True(result.TotalCount >= 0);
            // If your DB has data, verify the page size
            Assert.True(result.Data.Count() <= pageSize);
        }

        public void Dispose() => _dbScope.Dispose();
    }
}
