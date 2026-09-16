using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ApplicationTypes
{
    public class ApplicationTypeResponse
    {
        public byte ApplicationTypeID { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal ApplicationFees { get; set; }
    }
}
