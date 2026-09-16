using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountApplications
{
    public class AccountApplicationListItem
    {
        public int ApplicationID { get; set; }
        public int AccountID { get; set; }
        public byte ApplicationTypeID { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserID { get; set; }
    }
}
