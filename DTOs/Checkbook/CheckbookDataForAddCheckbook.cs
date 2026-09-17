using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Checkbook
{
    public class CheckbookDataForAddCheckbook
    {
        public int ApplicationID { get; set; }
        public byte NumberOfLeaves { get; set; }
        public int AccountID { get; set; }
    }
}
