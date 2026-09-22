using DTOs.Client;
using DTOs.Person;
using ServiceContract.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestBuilders
{
    public class ClientBuilder
    {
        private readonly ClientAddRequest _clientRequest;
        private readonly ClientUpdateRequest _clientUpdateRequest;
        public ClientBuilder(PersonAddRequest personRequest)
        {
            // Map base person fields into the specific ClientAddRequest
            _clientRequest = new ClientAddRequest
            {
                FirstName = personRequest.FirstName,
                LastName = personRequest.LastName,
                NationalId = personRequest.NationalId,
                Email = personRequest.Email,
                Phone = personRequest.Phone,
                BirthDate = personRequest.BirthDate,
                Gendor = personRequest.Gendor,
                ImagePath = personRequest.ImagePath,

                // Default client-specific values
                IsActive = true,
                RiskLevel = 1
            };
            _clientUpdateRequest = new ClientUpdateRequest();
        }

        public ClientBuilder WithRiskLevel(byte riskLevel)
        {
            _clientRequest.RiskLevel = riskLevel;

            _clientUpdateRequest.RiskLevel = riskLevel;
            return this;
        }

        public ClientBuilder WithIsActive(bool isActive)
        {
            _clientRequest.IsActive = isActive;

            _clientUpdateRequest.IsActive = isActive;
            return this;
        }

        // Terminal step: Seeds the database using the client write service
        public async Task<OperationResult<int>> BuildAsync(IClientWriteService clientService)
        {
            return await clientService.CreateAsync(_clientRequest, profileImage: null);
        }
        public async Task<OperationResult> UpdateAsync(IClientWriteService clientService)
        {
            return await clientService.UpdateAsync(_clientUpdateRequest, profileImage: null);
        }

    }
}
