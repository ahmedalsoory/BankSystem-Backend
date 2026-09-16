using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class AccountProductsQ
    {
        public static string GetByID = @"SELECT Id, Name, MinOpeningDeposit, ApplicationFee, InterestRate 
                  FROM [Banking].[AccountProducts] 
                  WHERE Id = @Id";
    }
}
