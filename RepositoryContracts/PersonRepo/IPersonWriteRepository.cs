using DTOs.Person;
using Shared;
using System.Data;


namespace RepositoryContracts.PersonRepo
{
    public interface IPersonWriteRepository
    {
        Task<OperationResult<int>> AddAsync(PersonAddRequest person);
        Task<OperationResult> UpdateAsync(PersonUpdateRequest person);
    }
}
