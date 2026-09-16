using DTOs.ApplicationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.AccountApplicationsType
{
    public interface IApplicationTypeWriteRepository
    {
        Task<bool> UpdateFeesAndDescriptionAsync(ApplicationTypeUpdateRequest request);
    }
}
