using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class CardTypeQ
    {
        public static string GetAll = @"select CardTypeID , TypeName , DefaultWithdrawalLimit,MaxWithdrawalLimit,
                                        RequiresManagerApproval,IsActive,RowVersion 
                                        from Ref.CardTypes";
    }
}
