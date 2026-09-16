using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.AccountApplicationsType
{
    public enum ApplicationType : byte
    {
        IssueCheckbook = 1,
        ReplaceCheckbook = 2,
        RenewCheckbook = 3,
        IssueLocalVisa = 4,
        IssueInternationalVisa = 5,
        ReplaceVisaCard = 6,
        RenewVisaCard = 7,
        LoanRequest = 8
    }
}
