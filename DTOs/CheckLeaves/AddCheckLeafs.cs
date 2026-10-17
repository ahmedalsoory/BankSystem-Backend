using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CheckLeaves
{
    public class AddCheckLeafs
    {
        public int CheckbookID { get; set; }
        public int AccountID { get; set; }
        public int BeginCheckNumber { get; set; }
        public int NumberOfLeaves { get; set; }
        public byte Status { get; set; }
    }
}
