using DTOs.Person;
using Microsoft.AspNetCore.Http;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Person
{
    public interface IPersonWriteService
    {
        Task<OperationResult<int>>AddAsync(PersonAddRequest request, IFormFile? profileImage
             );
        Task<OperationResult>UpdateAsync(PersonUpdateRequest request, IFormFile? profileImage
             );
    }
}
