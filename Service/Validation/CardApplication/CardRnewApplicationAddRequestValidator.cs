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
    public class CardRnewApplicationAddRequestValidator : AbstractValidator<CardRenewApplicationAddRequest>
    {
        public CardRnewApplicationAddRequestValidator()
        {
            // Include base class validation rules from CardApplicationAddRequest
            Include(new CardApplicationAddRequestValidator());

            // Ensure ApplicationTypeID matches the expected card renewal type
            RuleFor(x => x.ApplicationTypeID)
                .Equal((byte)ApplicationType.RenewVisaCard)
                .WithMessage("Invalid application type for a card renewal request.");

            // oldCardId must be a valid positive integer referencing the card being renewed
            RuleFor(x => x.oldCardId)
                .GreaterThan(0)
                .WithMessage("A valid old card ID is required for renewal.");
        }
    }
}
