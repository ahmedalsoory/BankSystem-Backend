using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.AccountApplications
{
    public class AccountApplicationResponse
    {
        public int ApplicationID { get; set; }
        public int AccountID { get; set; }
        public byte ApplicationTypeID { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public string? Notes { get; set; }
        [JsonConverter(typeof(SmartRowVersionConverter))]
        public byte[] RowVersion { get; set; } = null!;
    }
}
