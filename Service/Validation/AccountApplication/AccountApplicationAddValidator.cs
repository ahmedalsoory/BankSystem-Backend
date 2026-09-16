using DTOs.AccountApplications;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.AccountApplication
{
    public class AccountApplicationAddValidator : AbstractValidator<AccountApplicationAddRequest>
    {
        public AccountApplicationAddValidator()
        {
            RuleFor(x => x.AccountID)
                .GreaterThan(0)
                .WithMessage("A valid Account ID is required.");

            RuleFor(x => x.ApplicationTypeID)
                .NotEmpty()
                .WithMessage("Application Type is required.")
                .Must(typeId => typeId > 0 && typeId < 8)
                .WithMessage("Invalid Application Type ID.");

            // Optional: If you pass custom notes during submission
            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters.");

            RuleFor(x => x.CreatedByUserID)
                .GreaterThan(0)
                .WithMessage("A valid Creator User ID is required.");
        }
    }
}
