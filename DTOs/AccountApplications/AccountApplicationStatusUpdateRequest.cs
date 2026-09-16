using DTOs.interfaces;
using Shared.Enums.AccountApplications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTOs.AccountApplications
{
    public class AccountApplicationStatusUpdateRequest : IValidatableDto
    {
        public int ApplicationId { get; set; }
        public ApplicationStatus NewStatus { get; set; }
        public string? Notes { get; set; }
    }
}
