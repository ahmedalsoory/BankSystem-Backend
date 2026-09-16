using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class ConnectionString
    {
        public static string Connection =
             "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BankSystemDB;Max Pool Size=5" +
            ";Integrated Security=True;Connect Timeout=5;Encrypt=False;Trust Server Certificate=False" +
            ";Application Intent=ReadWrite;Multi Subnet Failover=False";
    }
}
