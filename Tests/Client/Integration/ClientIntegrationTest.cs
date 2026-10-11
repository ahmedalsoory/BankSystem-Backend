using DTOs.Client;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
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
        protected IClientWriteService _clientWriteService => _scopedServiceProvider.GetRequiredService<IClientWriteService>();
        protected IClientReadService _clientReadService => _scopedServiceProvider.GetRequiredService<IClientReadService>();
        protected IDbContextScope _connectionProvider => _dbContextScope;

        public ClientIntegrationTest(IntegrationTestFixture fixture) : base(fixture)
        {
            // Constructor stays clean. No manual scope creation here.
        }
    }
}
