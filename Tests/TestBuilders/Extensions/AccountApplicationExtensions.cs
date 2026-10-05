using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestBuilders.Extensions
{
    public static class AccountApplicationExtensions
    {
        public static IssueCardFirstTimeApplicationBuilder AsCardApplication(this AccountApplicationBuilder builder)
        {
            return new IssueCardFirstTimeApplicationBuilder(builder.GetRequest());
        }

        public static CardRenewApplicationBuilder AsRenewCardApplication(this AccountApplicationBuilder builder)
        {
            return new CardRenewApplicationBuilder(builder.GetRequest());
        }

        public static CardReplaceApplicationBuilder AsReplaceCardApplication(this AccountApplicationBuilder builder)
        {
            return new CardReplaceApplicationBuilder(builder.GetRequest());
        }
    }
}
