using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountApplications
{
    public interface IAccountApplicationReadTransactionRepository
    {
        Task<byte> GetApplicationTypeByIdAsync(int id);
    }
}
