using DTOs.CardApplication;
using FluentValidation;
using Shared.Enums.AccountApplicationsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.CardApplication
{
    public class CardReplaceApplicationAddRequestValidator : AbstractValidator<CardReplaceApplicationAddRequest>
    {
        public CardReplaceApplicationAddRequestValidator()
        {
            // Include base class rules from CardApplicationAddRequest (which also includes AccountApplicationAddRequest rules)
            Include(new CardApplicationAddRequestValidator());

            // Ensure ApplicationTypeID matches the expected card replacement type
            RuleFor(x => x.ApplicationTypeID)
                .Equal((byte)ApplicationType.ReplaceVisaCard)
                .WithMessage("Invalid application type for a card replacement request.");

            // oldCardId must be a valid positive integer referencing the card to be replaced
            RuleFor(x => x.oldCardId)
                .GreaterThan(0)
                .WithMessage("A valid old card ID is required for replacement.");
        }
    }
}
