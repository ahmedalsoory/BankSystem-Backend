
using ServiceContract;

    /*
namespace API.Middleware
{
    public class EntityGuardMiddleware
    {
        private readonly RequestDelegate _next;

        public EntityGuardMiddleware(RequestDelegate next)
        {
            _next = next;
            
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Identify if this is a request we care about (e.g., /api/clients/500)
            var path = context.Request.Path.Value?.Split('/');

            if (path != null && path.Length >= 4 && path[1] == "api")
            {
                string entityName = path[2].ToLower(); // e.g., "clients"
                if (int.TryParse(path[3], out int id))
                {
                    // 2. Map the URL string to our Generic Type T
                    // This is where our "T = Type = Generics" logic connects!
                    bool isAllowed = entityName switch
                    {
                        "clients" => _cache.IsActive(CacheType.Client, id),
                        "accounts" => _cache.IsActive(CacheType.AccountApplication,id),
                        _ => true // If we don't cache it, let it pass
                    };

                    // 3. THE BLOCK: If the bit is 0, we kill the request here
                    if (!isAllowed)
                    {
                        context.Response.StatusCode = 404; // Or 410 Gone
                        await context.Response.WriteAsync("Entity is inactive or deleted.");
                        return; // 🚀 STOP! We never hit the Database.
                    }
                }
            }

            // 4. If active, let the request continue to the Controller
            await _next(context);
        }
    }
}
*/