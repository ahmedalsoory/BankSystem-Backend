using DTOs.Person;
using FluentValidation;

namespace Service.Validation.Person
{
    public class PersonUpdateValidator : PersonCommonValidator<PersonUpdateRequest>
    {
        public PersonUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid person ID.");
        }
    }
}
