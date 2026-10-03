using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Service;
using ServiceContract.CardApplication;
using ServiceContract.OnboardingApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;

namespace Tests.OnboardingApplicationTypes.Integration
{
    public class OnboardingApplicationTypesServcieTest : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        protected IOnboardingApplicationTypesReadServcie _onboardingApplicationTypesReadServcie
            => _scopedServiceProvider.GetRequiredService<IOnboardingApplicationTypesReadServcie>();
        public OnboardingApplicationTypesServcieTest(IntegrationTestFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public async Task GetAll_WhenTypesExist_ShouldReturnApplicationTypes()
        {
            // Act
            var result = await _onboardingApplicationTypesReadServcie.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty(); // Assuming your test DB is seeded with lookup data
            MarkTestAsSuccessful();
        }
    }
}
