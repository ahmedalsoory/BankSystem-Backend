using DTOs.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Globle;
using Tests.TestBuilders;
using Tests.TestBuilders.Extensions;

namespace Tests.Client.Integration
{
    public partial class ClientIntegrationTest : IntegrationTestBase, IClassFixture<IntegrationTestFixture>
    {
        protected readonly IClientWriteService _clientWriteService;
        protected readonly IClientReadService _clientReadService;
        private readonly IDbConnectionProvider _connectionProvider;

        public ClientIntegrationTest(IntegrationTestFixture fixture) : base(fixture)
        {
            var scope = _serviceProvider.CreateScope();
            _connectionProvider = scope.ServiceProvider.GetRequiredService<IDbContextScope>();
            _clientWriteService = scope.ServiceProvider.GetRequiredService<IClientWriteService>();
            _clientReadService = scope.ServiceProvider.GetRequiredService<IClientReadService>();
        }
    }
}
