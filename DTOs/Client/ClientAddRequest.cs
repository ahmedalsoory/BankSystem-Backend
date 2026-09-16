
using DTOs.interfaces;
using DTOs.Person;
using DTOs.Person.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Client
{

    public class ClientAddRequest:PersonAddRequest
    {
        public byte RiskLevel { get; init; }
        public bool IsActive { get; init; }

        public string? ClientNumber { get; set; }
        public int? PersonID { get;  set; }



    }
}
