using DTOs.AccountApplications;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.AccountApplication
{
    public class AccountApplicationStatusUpdateValidator : AbstractValidator<AccountApplicationStatusUpdateRequest>
    {
        public AccountApplicationStatusUpdateValidator()
        {
            RuleFor(x => x.ApplicationId)
                .GreaterThan(0).WithMessage("A valid, positive ApplicationID is required.");


            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
