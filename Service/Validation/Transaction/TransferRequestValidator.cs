using DTOs.Transaction;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.Transaction
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            // 1. Include base transaction rules (Amount, Description, SourceId, etc.)
            Include(new TransactionBaseRequestValidator());

            // 2. Validate Transfer-specific properties
            RuleFor(x => x.FromAccountId)
                .GreaterThan(0)
                .WithMessage("Invalid source account ID.");

            RuleFor(x => x.ToAccountId)
                .GreaterThan(0)
                .WithMessage("Invalid destination account ID.")
                .NotEqual(x => x.FromAccountId)
                .WithMessage("Cannot transfer funds to the same account.");

            RuleFor(x => x.FromRowVersion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Sender row version is required to ensure fresh data concurrency.");
        }
    }
}
