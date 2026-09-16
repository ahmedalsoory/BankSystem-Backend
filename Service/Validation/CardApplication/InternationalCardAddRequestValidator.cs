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
    public class InternationalCardAddRequestValidator : AbstractValidator<InternationalCardAddRequest>
    {
        public InternationalCardAddRequestValidator()
        {
            // Include base class validation rules from CardApplicationAddRequest
            Include(new CardApplicationAddRequestValidator());

        }
    }
}
