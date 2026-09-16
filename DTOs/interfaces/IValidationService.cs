using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.interfaces
{
    public interface IValidationService<TContract>
    {
        Task<List<string>> ValidateAsync(TContract dto, int? id = null);
    }


}
