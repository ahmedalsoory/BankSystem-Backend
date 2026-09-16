using DTOs.AccountOpeningApplication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.AccountOpeningApplication
{
    public class AccountOpeningPagedRequestValidator : AbstractValidator<AccountOpeningPagedRequest>
    {
        public AccountOpeningPagedRequestValidator()
        {
            // PageNumber must be at least 1
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than or equal to 1.");

            // PageSize must be within a safe, reasonable performance range (e.g., between 1 and 100)
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100 items.");

            // Ensure SortBy is a valid enum value for AccountOpening
            RuleFor(x => x.SortBy)
                .IsInEnum()
                .WithMessage("Invalid sort field specified.");

            // Ensure Direction is a valid enum value
            RuleFor(x => x.Direction)
                .IsInEnum()
                .WithMessage("Invalid sort direction specified.");

            // If a FilterBy field is selected, a FilterValue must also be provided
            RuleFor(x => x.FilterValue)
                .NotEmpty()
                .When(x => x.FilterBy.HasValue)
                .WithMessage("Filter value cannot be empty when a filter field is selected.");

            // Prevent excessively long search queries from harming database performance
            RuleFor(x => x.FilterValue)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.FilterValue))
                .WithMessage("Filter value cannot exceed 100 characters.");
        }
    }
}
