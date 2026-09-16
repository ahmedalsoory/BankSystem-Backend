using DTOs.Client;
using ServiceContract.Client;

namespace API.Validation.OldVersion.Client
{
    public class ClientValidationLogic
    {
        public static async ValueTask<List<string>> Add(object dto, IClientValidationService svc)
        {
            var request = dto as ClientAddRequest;
            if (request == null) return new List<string> { "Invalid Request Type" };

            // 1. Run the Static Structural Checks (ClientValidator)
            var errors = ClientValidator.Validate(request);
            if (errors.Any()) return errors;

            // 2. Run the Database Checks (IClientValidationService)
            var dbErrors = await svc.ValidateAddAsync(request);

            return dbErrors;
        }
    }
}
