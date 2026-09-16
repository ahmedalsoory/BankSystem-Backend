using API.Validation.OldVersion.Person;
using DTOs.Client;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Validation.OldVersion.Client
{
    public static class ClientValidator
    {
        public static List<string> Validate(ClientAddRequest dto)
        {

            //if (dto.Person == null)
            //{
            //    return new List<string> { "Person data is required." };
            //}

            //var errors = PersonValidtor.Add(dto.Person);



            return new List<string>();
        }
    }
}
