using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CheckbookApplications
{
    public class CheckbookApplicationDetails
    {
        public int AccountID { get; set; }
        public byte NumberOfLeaves { get; set; } = 100;
        public int BeginCheckNumber { get; set; }

    }
}
