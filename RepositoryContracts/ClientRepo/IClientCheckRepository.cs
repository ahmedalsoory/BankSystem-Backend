using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.ClientRepo
{
    public interface IClientCheckRepository
    {
        Task<bool> HasDebtAsync(int clientId);
        Task<bool> IsEligibleForLoanAsync(int clientId);
    }
}
