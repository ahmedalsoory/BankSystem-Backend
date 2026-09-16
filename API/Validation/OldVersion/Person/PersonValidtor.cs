using DTOs.Person;
using Shared;
using System.Text.RegularExpressions;

namespace API.Validation.OldVersion.Person
{
    public class PersonValidtor
    {
        private static void ValidateCommonFields(PersonCommanFiled dto, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                errors.Add("First Name is required.");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                errors.Add("Last Name is required.");

            if (string.IsNullOrWhiteSpace(dto.NationalId))
                errors.Add("National ID is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");
            else if (!ValidationHelper.IsValidEmail(dto.Email))
                errors.Add("Email format is invalid.");

            if (string.IsNullOrWhiteSpace(dto.Phone) || dto.Phone.Length < 7)
                errors.Add("Phone number is required and must be valid.");

            if (dto.Gendor != 'M' && dto.Gendor != 'F')
                errors.Add("Gender must be 'M' or 'F'.");

            if (dto.BirthDate == default || dto.BirthDate > DateTime.Now)
                errors.Add("Valid Birth Date is required.");
            else if (DateTime.Now.Year - dto.BirthDate.Year < 10)
                errors.Add("The age should be greater than 10.");
        }

        // 2. Public Add Method
        public static List<string> Add(PersonAddRequest dto)
        {
            List<string> errors = new List<string>();
            if (dto == null) { errors.Add("Request body cannot be empty."); return errors; }

            ValidateCommonFields(dto, errors);
            return errors;
        }

        // 3. Public Update Method
        public static List<string> Update(PersonUpdateRequest dto)
        {
            List<string> errors = new List<string>();
            if (dto == null) { errors.Add("Request body cannot be empty."); return errors; }

            // Specific to Update
            if (dto.Id <= 0)
                errors.Add("Invalid person ID.");

            ValidateCommonFields(dto, errors);
            return errors;
        }

    }
}
