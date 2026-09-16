using DTOs.AccountApplications.interfaces;
using DTOs.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountApplications
{
    public class AccountApplicationAddRequest: IAccountApplicationValidationDTO
    {
        public int AccountID { get; set; }
        public virtual byte ApplicationTypeID { get; protected set; }
        public int CreatedByUserID { get; set; } = 1;
        public string? Notes { get; set; }
    }
}
