using DTOs.AccountOpeningApplication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.AccountOpeningApplication
{
    public class AccountOpeningApplicationAddRequestValidator : AbstractValidator<AccountOpeningApplicationAddRequest>
    {
        public AccountOpeningApplicationAddRequestValidator()
        {
            // ClientID must be a valid positive integer ID
            RuleFor(x => x.ClientID)
                .GreaterThan(0)
                .WithMessage("A valid Client ID is required.");

            // OnboardingTypeID must be provided (assuming 0 is an invalid/unassigned default)
            RuleFor(x => x.OnboardingTypeID)
                .GreaterThan((byte)0)
                .WithMessage("A valid Onboarding Type is required.");

            // CreatedByUserID must be a valid system user ID
            RuleFor(x => x.CreatedByUserID)
                .GreaterThan(0)
                .WithMessage("A valid User ID for the creator is required.");

            // Notes are optional, but if provided, limit their length to prevent database overflow
            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Notes))
                .WithMessage("Notes cannot exceed 500 characters.");

            // AccountType must be specified
            RuleFor(x => x.AccountType)
                .GreaterThan((byte)0)
                .WithMessage("A valid Account Type is required.");

            // CreatedDate cannot be set in the future, and shouldn't be an uninitialized default value
            RuleFor(x => x.CreatedDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("The creation date cannot be in the future.");

            // Currency code usually follows a 3-letter ISO format (e.g., USD, EUR) if provided
            RuleFor(x => x.Currency)
                .Length(3)
                .When(x => !string.IsNullOrEmpty(x.Currency))
                .WithMessage("Currency must be a valid 3-letter ISO code.")
                .Matches("^[A-Z]{3}$")
                .When(x => !string.IsNullOrEmpty(x.Currency))
                .WithMessage("Currency must consist of 3 uppercase letters.");

            // InitialDeposit must be non-negative (can be 0 or higher)
            RuleFor(x => x.InitialDeposit)
                .GreaterThanOrEqualTo(0f)
                .WithMessage("Initial deposit cannot be a negative value.");
        }
    }
}
