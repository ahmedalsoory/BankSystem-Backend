using AutoFixture;
using DTOs;
using DTOs.Client;
using Moq;
using RepositoryContracts.ClientRepo;
using ServiceContract.Client;
using Shared.Enums.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Shared.Enums;
using Service.Core;

namespace Tests.Client
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientReadRepository> _readRepoMock;
        private readonly Mock<IClientWriteRepository> _writeRepoMock;


        private readonly Lazy<IClientReadRepository> _lazyReadRepo;
        private readonly Lazy<IClientWriteRepository> _lazyWriteRepo;

        private readonly Fixture _fixture;
        private readonly ClientService _sut; // System Under Test

        public ClientServiceTests()
        {
            _fixture = new Fixture();
            _readRepoMock = new Mock<IClientReadRepository>();
            _writeRepoMock = new Mock<IClientWriteRepository>();

            // Setup Lazy wrappers to return the mocks
            _lazyReadRepo = new Lazy<IClientReadRepository>(() => _readRepoMock.Object);
            _lazyWriteRepo = new Lazy<IClientWriteRepository>(() => _writeRepoMock.Object);

            // Inject the lazy mocks into your service
            _sut = new ClientService(_lazyReadRepo, _lazyWriteRepo);
        }


        #region GetClientByID

        [Fact]
        public async Task GetByClientIDAsync_ShouldReturnClientDetail_WhenClientExists()
        {
            // Arrange
            int clientId = _fixture.Create<int>();
            var expectedDto = _fixture.Create<ClientDetailDto>();

            _readRepoMock.Setup(repo => repo.GetByPersonIDAsync(clientId))
                         .ReturnsAsync(expectedDto);

            // Act
            var result = await _sut.GetByClientIDAsync(clientId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
            _readRepoMock.Verify(repo => repo.GetByPersonIDAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task GetByClientIDAsync_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange - The "Negative Scenario" for your Dapper-based DTOs
            int clientId = _fixture.Create<int>();
            _readRepoMock.Setup(repo => repo.GetByPersonIDAsync(clientId))
                         .ReturnsAsync((ClientDetailDto?)null);

            // Act
            var result = await _sut.GetByClientIDAsync(clientId);

            // Assert
            result.Should().BeNull();
        }



        #endregion


        #region AddClient

        [Fact]
        public async Task CreateAsync_ShouldReturnTrue_WhenRegistrationSucceeds()
        {
            // Arrange
            var request = _fixture.Create<ClientAddRequest>();
            _writeRepoMock.Setup(repo => repo.RegisterClientAsync(request))
                          .ReturnsAsync(true);

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            result.Should().BeTrue();
            _writeRepoMock.Verify(repo => repo.RegisterClientAsync(request), Times.Once);
        }
        #endregion


        #region GetClientPaging

        [Fact]
        public async Task GetClientsAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var expectedResult = _fixture.Create<PagedResult<ClientListItemDto>>();

            _readRepoMock.Setup(repo => repo.GetClientsPagedAsyncAsList(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SortedBy_Client>(),
                It.IsAny<Direction>(), It.IsAny<Filter_Client?>(), It.IsAny<string?>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await ((IClientReadService)_sut).GetClientsAsync(1, 10, SortedBy_Client.Name,
                Direction.ACS);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(expectedResult.TotalCount);

            _readRepoMock.Verify(repo => repo.GetClientsPagedAsyncAsList(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SortedBy_Client>(),
                It.IsAny<Direction>(), It.IsAny<Filter_Client?>(), It.IsAny<string?>()), Times.Once);
        }

        #endregion
    }
}
