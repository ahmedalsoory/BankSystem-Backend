using DTOs;
using DTOs.Client;
using Shared.Enums;
using Shared.Enums.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Client
{
    public interface IClientReadService
    {
        Task<ClientResponse?> GetByAccountNumberAsync(string accountNumber);
        Task<ClientDetailDto?> GetByClientIDAsync(int Id);
        Task<PagedResult<ClientListItemDto>> GetClientsAsync(
     ClientPagedRequest request);

    }
}
