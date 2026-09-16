using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountRepo
{
    public interface IAccountLockRepository
    {
        Task LockAccountForTransferAsync(int fromAccountId, int toAccountId);
    }
}
