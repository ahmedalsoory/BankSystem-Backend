using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountRepo
{
    public interface IAccountCheckRepository
    {
        Task<bool> HasReachedLimitAsync(int clientId);
        Task<bool> IsActiveAsync(int accountId);
    }
}
