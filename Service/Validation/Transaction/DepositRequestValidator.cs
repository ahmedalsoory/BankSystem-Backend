using DTOs.Transaction;
using FluentValidation;
using Shared.Enums.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.Transaction
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            // 1. Validate Account ID
            RuleFor(x => x.ToAccountId)
                .GreaterThan(0)
                .WithMessage("A valid Target Account ID is required.");

            // 2. Validate Transaction Type matches Deposit
            RuleFor(x => x.Type)
                .Equal(enTransactionType.Deposit)
                .WithMessage("Invalid transaction type for deposit.");

            // 3. Inherited properties from TransactionBaseRequest (e.g., Amount)
            // Adjust property names based on your actual base class definition
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Deposit amount must be greater than zero.");

            // 4. Optional: Validate RowVersion if it's sent from the client
            // RuleFor(x => x.RowVersion)
            //     .NotNull()
            //     .NotEmpty()
            //     .When(x => x.RowVersion != null); // Only validate if required by your flow
        }
    }
}
