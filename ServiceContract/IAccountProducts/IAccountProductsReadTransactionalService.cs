using DTOs.AccountProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.IAccountProducts
{
    public interface IAccountProductsReadTransactionalService
    {
        Task<AccountProductsResponse?> GetByIDAsyncTransactional(int id);
    }
}
