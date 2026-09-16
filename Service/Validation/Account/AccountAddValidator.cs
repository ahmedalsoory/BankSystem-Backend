using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.Account
{
    using DTOs.Account;
    using FluentValidation;

    public class AccountAddValidator : AbstractValidator<AccountAddRequest>
    {
        public AccountAddValidator()
        {
            RuleFor(x => x.ClientID)
                .NotEmpty();

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3).WithMessage("Currency must be exactly 3 characters (e.g. USD).")
                .Must(c => c == c.ToUpper()).WithMessage("Currency must be uppercase.");

            //RuleFor(x => x.InitialDeposit)
            //    .GreaterThanOrEqualTo(0).WithMessage("You cannot start an account with a negative balance.");

            RuleFor(x => x.AccountType)
                .InclusiveBetween((byte)1, (byte)5).WithMessage("Invalid Account Type selection.");

            RuleFor(x => x.CreatedByUserID)
                .NotEmpty().WithMessage("Employee ID is required for auditing.");
        }
    }
}
