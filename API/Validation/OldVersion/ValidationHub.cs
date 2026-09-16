using API.Validation.OldVersion.Client;
using ServiceContract.Client;

namespace API.Validation.OldVersion
{
    public static class ValidationHub
    {
        public static class Client
        {
            public static async ValueTask<List<string>> Add(object dto, IClientValidationService svc)
                => await ClientValidationLogic.Add(dto, svc);


        }
    }
}
