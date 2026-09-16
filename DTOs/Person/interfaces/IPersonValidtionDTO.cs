using DTOs.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Person.interfaces
{
    public interface IPersonValidtionDTO 
    {
        string NationalId { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
    }
}
