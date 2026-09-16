using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Checkbook
{
    public class CheckbookAddRequest
    {
        public int AccountID { get; set; }
        public int ApplicationID { get; set; }
        public byte NumberOfLeaves { get; set; } = 100;
    }
}
