using Azure.Core;
using DTOs;
using DTOs.Client;

using DTOs.interfaces;
using DTOs.Person;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RepositoryContracts.ClientRepo;
using ServiceContract;
using ServiceContract.Client;
using ServiceContract.Person;
using Shared;
using Shared.Enums;
using Shared.Enums.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core
{
    public class ClientService : IClientWriteService, IClientReadService
    {


        private readonly IClientReadRepository _clientReadRepository;
        private readonly IClientWriteRepository _clientWriteRepository;
        private readonly Context _context;
        private readonly IPersonWriteService _personWriteService;

        public ClientService(IClientReadRepository clientReadRepository,
            IClientWriteRepository clientWriteRepository, IPersonWriteService personWriteService
             ,Context context)
        {

            _clientReadRepository = clientReadRepository;
            _clientWriteRepository = clientWriteRepository;
            _personWriteService = personWriteService;
            _context= context;
        }

        public async Task<ClientDetailDto?> GetByClientIDAsync(int Id)
        {

            return await _clientReadRepository.GetByPersonIDAsync(Id);
        }
        public async Task<OperationResult<int>> CreateAsync(ClientAddRequest registerClientRequest
            , IFormFile? profileImage)
        {
            _context.ImageFolderName = "client";
            OperationResult<int> result = await _personWriteService.AddAsync(registerClientRequest
                , profileImage);

            if (!result.Success)
            {
                return result;
            }

            registerClientRequest.PersonID = result.Data;

            string? clientNumber = await _clientReadRepository.GetNextClientNumberAsync();

            if (clientNumber == null)
            {
                return OperationResult<int>.Failure("Client number not genreate");
            }

            registerClientRequest.ClientNumber = clientNumber;


            OperationResult clientResult=
                await _clientWriteRepository.RegisterClientAsync(registerClientRequest);

            if(!clientResult.Success) return OperationResult<int>.Failure(clientResult.Errors);

            return result;
        }



        public async Task<ClientResponse?> GetByAccountNumberAsync(string accountNumber)
        {

            return await _clientReadRepository.GetByAccountNumberAsync(accountNumber);

        }

        async Task<PagedResult<ClientListItemDto>> IClientReadService.GetClientsAsync(ClientPagedRequest request)
        {

            return await _clientReadRepository.GetClientsPagedAsyncAsList(request);

        }

        public async Task<OperationResult> UpdateAsync(ClientUpdateRequest registerClientRequest
            , IFormFile? profileImage)
        {
            _context.ImageFolderName = "client";
            OperationResult result = await _personWriteService.UpdateAsync(registerClientRequest
                ,profileImage);

            if (!result.Success)
            {
                return result;
            }

            return await _clientWriteRepository.UpdateClientAsync(registerClientRequest);
        }
    }
}
