
using DTOs.interfaces;
using DTOs.Person;
using DTOs.Person.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Client
{
    public class ClientUpdateRequest : PersonUpdateRequest,IValidatableDto, IVersioned
    {
        public byte RiskLevel { get; set; }
        public bool IsActive { get; set; }

        [JsonConverter(typeof(SmartRowVersionConverter))]
        public byte[] ClientRowVersion { get; set; } = new byte[0];
        byte[] IVersioned.RowVersion => ClientRowVersion;

    }
}
