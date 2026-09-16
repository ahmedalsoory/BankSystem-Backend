using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Person.interfaces
{
    public interface IPersonSchema
    {
        string NationalId { get; }
        string Email { get; }
        string Phone { get; }
    }
}
