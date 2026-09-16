using DTOs.Transaction;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.Transaction
{
    public class TransactionBaseRequestValidator : AbstractValidator<TransactionBaseRequest>
    {
        public TransactionBaseRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Transaction amount must be greater than zero.");

            RuleFor(x => x.Description)
                .MaximumLength(250)
                .WithMessage("Description cannot exceed 250 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.SourceId)
                .GreaterThan(0)
                .WithMessage("Invalid Source ID.")
                .When(x => x.SourceId.HasValue);

            RuleFor(x => x.sourceType)
                .IsInEnum()
                .WithMessage("Invalid source type specification.")
                .When(x => x.sourceType.HasValue);
        }
    }
}
