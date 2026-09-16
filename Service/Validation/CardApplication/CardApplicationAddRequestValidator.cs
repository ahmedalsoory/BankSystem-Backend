using DTOs.CardApplication;
using FluentValidation;
using Service.Validation.AccountApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation.CardApplication
{
    public class CardApplicationAddRequestValidator : AbstractValidator<CardApplicationAddRequest>
    {
        public CardApplicationAddRequestValidator()
        {
            // Include base class validation rules for AccountApplicationAddRequest
            Include(new AccountApplicationAddValidator());

            // Ensure ApplicationTypeID is correctly set for local visa card applications
            RuleFor(x => x.ApplicationTypeID)
                .Equal((byte)Shared.Enums.AccountApplicationsType.ApplicationType.IssueLocalVisa)
                .WithMessage("Invalid application type for a card request.");

           
        }
    }
}
