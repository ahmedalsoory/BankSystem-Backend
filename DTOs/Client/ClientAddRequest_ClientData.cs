using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Client
{

        public class ClientAddRequest_ClientData
        {


        // Used for your logic to prioritize or limit certain clients
        public byte RiskLevel { get; init; } 
        public bool IsActive { get; init; } 

        public string? ClientNumber { get; set; }
        public int? PersonID { get; set; }

        public ClientAddRequest_ClientData() { }

        // Optional: A parameterless constructor for the JSON Serialize
        public ClientAddRequest_ClientData( byte RiskLeve = 1, bool IsActive = true)
            {

                this.RiskLevel = RiskLeve;
                this.IsActive = IsActive;

            }
        public ClientAddRequest_ClientData(string? ClientNumber,int PersonID,
            byte RiskLeve = 1, bool IsActive = true)
        {
            this.ClientNumber = ClientNumber;
            this.RiskLevel = RiskLeve;
            this.IsActive = IsActive;
            this.PersonID = PersonID;

        }
    }
    }

