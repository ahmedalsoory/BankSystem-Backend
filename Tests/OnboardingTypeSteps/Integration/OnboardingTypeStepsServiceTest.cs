using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract.IOnboardingTypeSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;

namespace Tests.OnboardingTypeSteps.Integration
{
    public class OnboardingTypeStepsServiceTest : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        protected IOnboardingTypeStepsReadService _onboardingTypeStepsReadService
            => _scopedServiceProvider.GetRequiredService<IOnboardingTypeStepsReadService>();

        public OnboardingTypeStepsServiceTest(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetAll_WhenStepsExist_ShouldReturnOnboardingSteps()
        {
            // Act
            var result = await _onboardingTypeStepsReadService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            MarkTestAsSuccessful();
        }
    }
}
