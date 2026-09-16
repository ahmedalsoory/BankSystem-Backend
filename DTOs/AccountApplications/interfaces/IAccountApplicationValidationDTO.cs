using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountApplications.interfaces
{
    public interface IAccountApplicationValidationDTO
    {
        int AccountID { get; }
        byte ApplicationTypeID { get; }
    }
}
