using DTOs.Person.interfaces;
using RepositoryContracts.PersonRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation
{
    //public class PersonValidationService : IPersonValidationService
    //{
    //    private readonly IPersonValidationRepository _repo;

    //    public PersonValidationService(IPersonValidationRepository repo)
    //    {
    //        _repo = repo;
    //    }

    //    public async Task<List<string>> ValidateIdentityAsync(IPersonSchema dto, int? personId = null)
    //    {
    //        1.Call the Repo(Infrastructure Layer)
    //        var conflicts = await _repo.GetConflictReasonsAsync(
    //            dto.NationalId,
    //            dto.Email,
    //            dto.Phone,
    //            personId);

    //        2.Business Logic: Map codes to user-friendly messages
    //        List<string> errors = new();
    //        foreach (var reason in conflicts)
    //        {
    //            errors.Add(reason switch
    //            {
    //                "NationalId" => "National ID is already in use.",
    //                "Email" => "Email address is already registered.",
    //                "Phone" => "Phone number is already linked to an account.",
    //                _ => "Unknown identity conflict."
    //            });
    //        }
    //        return errors;
    //    }
    //}
}
