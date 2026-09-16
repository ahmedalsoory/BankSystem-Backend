using System.Collections.Concurrent;

namespace API.Middleware
{
    public class ShieldMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly ConcurrentDictionary<string, int> _requestTracker = new();
        private static DateTime _lastReset = DateTime.UtcNow;

        public ShieldMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. CLEANUP: If 1 minute passed, reset counters to save 86MB RAM
            if ((DateTime.UtcNow - _lastReset).TotalMinutes > 1)
            {
                _requestTracker.Clear();
                _lastReset = DateTime.UtcNow;
            }

            // 2. FINGERPRINTING: Combine IP and UserAgent
            string ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var headers = context.Request.Headers;

            // 1. Create a "Structural Hash" (The order/presence of headers, NOT their values)
            // This ignores the changing User-Agent text and looks at the "Shape" of the request.
            int structuralHash = 0;
            foreach (var header in headers)
            {
                structuralHash ^= header.Key.GetHashCode();
            }

            string fingerprint = $"{ip}_{structuralHash}";

            // 2. PROTECT THE RAM: Maximum Dictionary Size
            // If the attacker is spamming new fingerprints, we stop adding them to the RAM.
            if (_requestTracker.Count > 5000)
            {
                _requestTracker.Clear(); // Emergency flush if RAM is getting full
            }

            // 3. THE "SUSPICIOUS" CHECK
            // If the User-Agent is missing or weirdly short, it's a bot.
            string ua = headers["User-Agent"].ToString();
            if (ua.Length < 10)
            {
                context.Response.StatusCode = 403;
                return;
            }

            var count = _requestTracker.AddOrUpdate(fingerprint, 1, (key, old) => old + 1);

            if (count > 20)
            {
                context.Response.StatusCode = 429;
                return;
            }

            await _next(context); 
        }
    }
}
