using DTOs.ApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountApplicationsType
{
    public interface IApplicationTypeReadTransactionService
    {
        Task<ApplicationTypeResponse?> GetByIdTransactionAsync(byte id);
    }
}
