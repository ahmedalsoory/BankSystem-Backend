using DTOs.Client;
using DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Client
{
    public interface IClientValidationService
    {
        Task<List<string>> ValidateAddAsync(ClientAddRequest request);
    }
}
