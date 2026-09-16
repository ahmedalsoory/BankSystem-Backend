
using DTOs.interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Client
{

  

    public class ClientUpdateRequest_ClientData : IVersioned,IHasId
    {
        public int Id { get; set; }
        public byte RiskLevel { get; init; }
        public bool IsActive { get; init; }
        public byte[] ClientRowVersion { get; init; } = new byte[0];

        // Explicitly implement the IVersioned interface requirement
        byte[] IVersioned.RowVersion => ClientRowVersion;
        // Optional: A parameterless constructor for the JSON Serialize
        [JsonConstructor]
        public ClientUpdateRequest_ClientData(int Id,
            byte RiskLevel, bool IsActive, byte[] rowVersion)
        {
            this.Id = Id;
            this.RiskLevel = RiskLevel;
            this.IsActive = IsActive;

            // The logic lives here! It handles the "0x" or Base64 automatically
            this.ClientRowVersion = rowVersion;
        }

      
    }
}
