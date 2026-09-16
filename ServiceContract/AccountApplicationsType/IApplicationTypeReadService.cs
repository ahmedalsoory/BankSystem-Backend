using DTOs.ApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.AccountApplicationsType
{
    public interface IApplicationTypeReadService
    {
        Task<ApplicationTypeResponse?> GetByIdAsync(byte id);
        Task<IEnumerable<ApplicationTypeResponse>> GetAllAsync();
    }
}
