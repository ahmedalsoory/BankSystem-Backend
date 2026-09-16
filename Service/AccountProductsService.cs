using DTOs.AccountProducts;
using Microsoft.Extensions.Logging;
using RepositoryContracts.AccountProducts;
using ServiceContract.IAccountProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AccountProductsService : BaseService<AccountProductsService>
        , IAccountProductsReadTransactionalService
    {
        private readonly IAccountProductsReadTransactionalRepository _readTransactionalRepository;
        public AccountProductsService(IAccountProductsReadTransactionalRepository readTransactionalRepository,
            ILogger<AccountProductsService> logger) : base(logger)
        {
            _readTransactionalRepository = readTransactionalRepository;
        }

        public async Task<AccountProductsResponse?> GetByIDAsyncTransactional(int id)
        {
            return await _readTransactionalRepository.GetByIDAsyncTransactional(id);
        }
    }
}
