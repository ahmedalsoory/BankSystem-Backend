using AutoFixture;
using DTOs.Client;
using DTOs.interfaces;
using DTOs.Person;
using DTOs.Person.interfaces;
using Moq;
using RepositoryContracts.ClientRepo;
using Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Client
{
    /*
    public class ClientValidationServiceTests
    {
        // 1. Mock the Generic Interface instead of the specific Repo
        private readonly Mock<IValidationService<IPersonValidtionDTO>> _personRepoMock;
        private readonly ClientValidationService _sut;
        private readonly Fixture _fixture;

        public ClientValidationServiceTests()
        {
            _fixture = new Fixture();
            _personRepoMock = new Mock<IValidationService<IPersonValidtionDTO>>();

            // 2. Inject the new mock into the service
            _sut = new ClientValidationService(_personRepoMock.Object);
        }

        [Fact]
        public async Task ValidateAsync_ShouldReturnError_WhenNationalIdExists()
        {
            // Arrange
            var request = _fixture.Create<ClientAddRequest>();

            // 3. Setup the Generic ValidateAsync method
            _personRepoMock.Setup(r => r.ValidateAsync(
                        It.IsAny<IPersonValidtionDTO>(),
                        It.IsAny<int?>()))
                     .ReturnsAsync(new List<string> { "NationalId" });

            // Act
            var result = await _sut.ValidateAsync(request);

            // Assert
            Assert.Contains("NationalId", result);
        }

        [Theory]
        [InlineData("NationalId")]
        [InlineData("Email")]
        [InlineData("Phone")]
        public async Task ValidateAsync_ShouldReturnCorrectConflicts(string dbReason)
        {
            // Arrange
            var request = _fixture.Create<ClientAddRequest>();

            _personRepoMock.Setup(r => r.ValidateAsync(
                        It.IsAny<IPersonValidtionDTO>(),
                        It.IsAny<int?>()))
                     .ReturnsAsync(new List<string> { dbReason });

            // Act
            var result = await _sut.ValidateAsync(request);

            // Assert
            Assert.Contains(dbReason, result);
        }
    }
    */
}
