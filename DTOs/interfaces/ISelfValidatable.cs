using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.interfaces
{
    public interface ISelfValidatable
    {
        /// <summary>
        /// Executes static validation rules and returns a list of error messages.
        /// Returns an empty list if validation passes.
        /// </summary>
        List<string> GetStaticErrors();
    }
}
