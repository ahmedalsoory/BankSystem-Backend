using DTOs.Person;
using FluentValidation;

namespace Service.Validation.Person
{
    public class PersonCommonValidator<T> : AbstractValidator<T> where T : PersonCommanFiled
    {
        public PersonCommonValidator()
        {
         
            // Replaces: if (string.IsNullOrWhiteSpace(dto.FirstName))
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.");

            // Replaces: if (string.IsNullOrWhiteSpace(dto.Email)) + Regex check
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            // Replaces: if (string.IsNullOrWhiteSpace(dto.Phone) || dto.Phone.Length < 7)
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .MinimumLength(7).WithMessage("Phone number must be at least 7 characters.");

            // Replaces: if (dto.Gendor != 'M' && dto.Gendor != 'F')
            RuleFor(x => x.Gendor)
                .Must(g => g == 'M' || g == 'F').WithMessage("Gender must be 'M' or 'F'.");

            // Replaces: if (dto.BirthDate == default || dto.BirthDate > DateTime.Now)
            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Valid Birth Date is required.")
                .LessThan(DateTime.Now).WithMessage("Birth Date cannot be in the future.")
                .Must(date => DateTime.Now.Year - date.Year >= 10).WithMessage("The age should be greater than 10.");
        }
    }
}
