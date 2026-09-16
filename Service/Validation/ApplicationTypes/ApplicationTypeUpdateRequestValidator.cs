using DTOs.ApplicationTypes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.ApplicationTypes
{
    public class ApplicationTypeUpdateRequestValidator : AbstractValidator<ApplicationTypeUpdateRequest>
    {
        public ApplicationTypeUpdateRequestValidator()
        {
            RuleFor(x => x.ApplicationTypeID)
                .NotEmpty()
                .WithMessage("Application Type ID is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty.")
                .MaximumLength(250)
                .WithMessage("Description cannot exceed 250 characters.");

            RuleFor(x => x.ApplicationFees)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Application fees cannot be negative.");
        }
    }
}
