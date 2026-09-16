using DTOs.Client;
using DTOs.Person;
using Microsoft.AspNetCore.Http;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Client
{
    public  interface IClientWriteService
    {
        Task<OperationResult<int>> CreateAsync(ClientAddRequest registerClientRequest, IFormFile? profileImage);
        Task<OperationResult> UpdateAsync(ClientUpdateRequest registerClientRequest, IFormFile? profileImage);
    }
}
