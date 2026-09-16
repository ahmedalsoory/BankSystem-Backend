using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.PersonRepo
{
    public interface IPersonCheckRepository
    {
        Task<bool> IsNationalIdUniqueAsync(string nationalId);
        Task<bool> IsAdultAsync(int personId);
    }
}
