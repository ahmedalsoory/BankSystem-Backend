using DTOs.Client;
using FluentValidation;
using Service.Validation.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.Client
{
   public class ClientUpdateValidator : AbstractValidator<ClientUpdateRequest>
    {
        public ClientUpdateValidator()
        {
            // 1. Validate the Nested Person Object
            // This tells FluentValidation: "Use the PersonAddValidator to check the Person property"
            //RuleFor(x => x.Person)
            //    .NotNull().WithMessage("Person data is required.")
            //    .SetValidator(new PersonUpdateValidator());

            //// 2. Validate the Client-Specific Struct Data
            //RuleFor(x => x.ClientDetails)
            //    .NotNull().WithMessage("Client details are required.");

            //RuleFor(x => x.ClientDetails.RiskLevel)
            //    .InclusiveBetween((byte)1, (byte)5)
            //    .WithMessage("Risk level must be between 1 and 5.");
        }
    }
}
