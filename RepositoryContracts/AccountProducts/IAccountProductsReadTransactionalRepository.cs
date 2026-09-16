using DTOs.AccountProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountProducts
{
    public interface IAccountProductsReadTransactionalRepository
    {
        Task<AccountProductsResponse?> GetByIDAsyncTransactional(int id);
    }
}
