using DTOs.AccountOpeningApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountOpeningApplications
{
    public interface IAccountOpeningApplicationsReadTransactionService
    {
        Task<AccountOpeningApplicationResponse?> GetByIdAsyncTransactional(int id);
    }
}
