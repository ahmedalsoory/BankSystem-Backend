using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.AccountOpeningDetail
{
    using DTOs.AccountOpeningDetail;
    using FluentValidation;

    public class AccountOpeningDetailRequestValidator : AbstractValidator<AccountOpeningDetailRequest>
    {
        public AccountOpeningDetailRequestValidator()
        {
            // ApplicationID must be a valid positive integer linking back to the parent application
            RuleFor(x => x.ApplicationID)
                .GreaterThan(0)
                .WithMessage("A valid Application ID is required.");

            // RequirementKey must be specified so the system knows what field is being saved
            RuleFor(x => x.RequirementKey)
                .NotEmpty()
                .WithMessage("Requirement key cannot be empty.")
                .MaximumLength(100)
                .WithMessage("Requirement key cannot exceed 100 characters.");
                

            // RequirementValue is required (can be empty string if intentionally cleared, but must not be null)
            RuleFor(x => x.RequirementValue)
                .NotNull()
                .WithMessage("Requirement value cannot be null.")
                .MaximumLength(2000)
                .When(x => !string.IsNullOrEmpty(x.RequirementValue))
                .WithMessage("Requirement value cannot exceed 2000 characters.");
        }
    }
}
