using FluentAssertions;
using Service.Core;
using System.Threading.Tasks;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;
using Xunit;

namespace Tests.Client.Integration
{
    public partial class ClientIntegrationTest
    {
        [Fact]
        public async Task CreateAsync_WhenBuildingClient_ShouldSucceed()
        {
            // Arrange & Act
            var result = await new PersonBuilder()
                .WithName("Khaled", "Youssef")
                .AsClient()
                .WithRiskLevel(2)
                .WithIsActive(true)
                .BuildAsync(_clientWriteService);

            // Assert success first
            result.Success.Should().BeTrue($"because: {string.Join(", ", result.Errors)}");
            result.Data.Should().BeGreaterThan(0);

            // Mark the test as successful so the transaction pipeline commits automatically on dispose
            MarkTestAsSuccessful();
        }
        [Fact]
        public async Task CreateClient_WhenEmailAlreadyExists_ShouldFailConstraint()
        {
            // Arrange & Act
            var result = await new PersonBuilder()
                .Create(s => s.Fail(PersonFailureType.EmailAlreadyExists))
                 .AsClient()
                .BuildAsync(_clientWriteService);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Email");
        }
        [Fact]
        public async Task CreateClient_WhenPhoneAlreadyExists_ShouldFailConstraint()
        {
            // Arrange & Act
            var result = await new PersonBuilder()
                .Create(s => s.Fail(PersonFailureType.PhoneAlreadyExists))
                .AsClient()
              
                .BuildAsync(_clientWriteService);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Phone");
        }
        [Fact]
        public async Task CreateClient_WhenNationalIdIsInvalid_ShouldFailConstraint()
        {
            // Arrange & Act
            var result = await new PersonBuilder()
                .Create(s => s.Fail(PersonFailureType.InvalidNationalId))
                .AsClient()
                .BuildAsync(_clientWriteService);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("NationalId");
        }

    }
}