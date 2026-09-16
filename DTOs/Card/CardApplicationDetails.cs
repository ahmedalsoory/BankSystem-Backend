using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Card
{
    public class CardApplicationDetails
    {
        public int AccountID { get; set; }
        public byte CardTypeID { get; set; }
        public string ClientFullName { get; set; } = string.Empty;
    }
}
