using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests.Client.Load
{
    public partial class ClientLoadTests
    {
        protected readonly ITestOutputHelper _output;

        public ClientLoadTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // Shared helper methods across your load tests can go here if needed
    }
}
