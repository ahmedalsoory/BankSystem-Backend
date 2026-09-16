using DTOs.AccountApplications;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.AccountApplication
{
    public class AccountApplicationPagedRequestValidator : AbstractValidator<AccountApplicationPagedRequest>
    {
        public AccountApplicationPagedRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be 1 or greater.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).When(x => x.AccountId.HasValue)
                .WithMessage("If provided, AccountId must be a positive integer.");
        }
    }
}
