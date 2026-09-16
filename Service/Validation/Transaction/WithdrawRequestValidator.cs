using DTOs.Transaction;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.Transaction
{
    public class WithdrawRequestValidator : AbstractValidator<WithdrawRequest>
    {
        public WithdrawRequestValidator()
        {
            // 1. Include base transaction rules (Amount, Description, SourceId, etc.)
            Include(new TransactionBaseRequestValidator());

            // 2. Validate Withdrawal-specific properties
            RuleFor(x => x.FromAccountId)
                .GreaterThan(0)
                .WithMessage("Invalid account ID.");

            RuleFor(x => x.RowVersion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Account row version is required to ensure data concurrency.");
        }
    }
}
