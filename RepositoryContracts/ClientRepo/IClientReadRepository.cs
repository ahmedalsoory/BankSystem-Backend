
using DTOs;
using DTOs.Client;
using Shared.Enums;
using Shared.Enums.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.ClientRepo
{
    public interface IClientReadRepository
    {
        Task<ClientResponse?> GetByAccountNumberAsync(string accountNumber);
        Task<ClientDetailDto?> GetByPersonIDAsync(int Id);

        Task<string> GetNextClientNumberAsync();
        public Task<PagedResult<ClientListItemDto>> GetClientsPagedAsyncAsList(
      ClientPagedRequest request);

    }
}
