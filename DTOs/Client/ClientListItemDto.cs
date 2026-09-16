using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Client
{
    public class ClientListItemDto
    {
        public int PersonID {  get; set; }
        public string FirstName { get; set; }= string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime JoinedDate { get; set; }
        public string ClientNumber { get; set; } = string.Empty;
    }
}
