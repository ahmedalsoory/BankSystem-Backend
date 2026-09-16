using DTOs.ApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountApplicationsType
{
    public interface IApplicationTypeReadRepository
    {
        Task<ApplicationTypeResponse?> GetByIdAsync(byte id);
        Task<IEnumerable<ApplicationTypeResponse>> GetAllAsync();
    }
}
