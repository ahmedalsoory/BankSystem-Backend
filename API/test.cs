using RepositoryContracts.PersonRepo;
using ServiceContract;
using DTOs.interfaces;

namespace API
{
    public class test
    {
        public class ValidationService<T,TServies> where TServies : IValidationService<T>
        {
            private readonly TServies _repo;

            public ValidationService(TServies personRepo)
            {
                _repo = personRepo;
            }

            public async ValueTask<List<string>> ValidateAsync(T dto)
            {
                List<string> errors = new();

                // 1. Extract ID and RowVersion using Pattern Matching
                int? id = (dto is IHasId hasId) ? hasId.Id : null;

                if (dto is IVersioned versioned && (versioned.RowVersion == null || versioned.RowVersion.Length == 0))
                {
                    errors.Add("Concurrency token (RowVersion) is missing.");
                    return errors; // Early exit (Fast Path)
                }

                // 2. Validate Person Data (If the DTO has it)
            
                    // Call the DB once to check NationalId, Email, and Phone
                    // If id has a value, the SQL will ignore the current record (Self-Exclusion)
                    var personErrors = await _repo.ValidateAsync(dto, id);
                    errors.AddRange(personErrors);
                

                // 3. Add more table-specific logic here as needed

                return errors;
            }
        }
    }
}
