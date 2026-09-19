using API.Validation.OldVersion.Person;
using DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Person
{
    /*
    public class PersonValidatorTests
    {
        [Theory]
        // 1. Test First Name
        [InlineData("", "ValidLastName", "N123", "test@gmail.com", "123456789", 'M', "First Name is required.")]
        // 2. Test Invalid Email
        [InlineData("Ali", "ValidLastName", "N123", "bad-email", "123456789", 'M', "Email format is invalid.")]
        // 3. Test Short Phone
        [InlineData("Ali", "ValidLastName", "N123", "test@gmail.com", "123", 'M', "Phone number is required and must be valid.")]
        // 4. Test Invalid Gender
        [InlineData("Ali", "ValidLastName", "N123", "test@gmail.com", "123456789", 'X', "Gender must be 'M' or 'F'.")]
        public void Add_ShouldReturnExpectedError_WhenDataIsInvalid(
         string fName, string lName, string nationalId, string email, string phone, char gender, string expectedError)
        {
            // Arrange
            var dto = new PersonAddRequest
            {
                FirstName = fName,
                LastName = lName,
                NationalId = nationalId,
                Email = email,
                Phone = phone,
                Gendor = gender,
                BirthDate = new DateTime(1990, 1, 1) // Provide valid age to avoid age errors
            };

            // Act
            var errors = PersonValidtor.Add(dto);

            // Assert
            Assert.Contains(expectedError, errors);
        }
    }
    */
}
