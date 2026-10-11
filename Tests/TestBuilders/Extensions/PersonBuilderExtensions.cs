using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestBuilders.Extensions
{
    public static class PersonBuilderExtensions
    {
        public static ClientBuilder AsClient(this PersonBuilder builder)
        {
            return new ClientBuilder(builder.GetRequest());
        }

        // Later, you can easily add:
        // public static EmployeeBuilder AsEmployee(this PersonBuilder builder) => new EmployeeBuilder(builder.GetRequest());
    }
}
