using DTOs.interfaces;
using DTOs.Person.interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.Person
{
    public class PersonUpdateRequest : PersonCommanFiled , IHasId, IVersioned, IPersonValidtionDTO
    {
        public int Id { get; set; }
        [JsonConverter(typeof(SmartRowVersionConverter))]
        public byte[] PersonRowVersion { get; set; } =new byte[0];
        byte[] IVersioned.RowVersion => PersonRowVersion;



    }
}
