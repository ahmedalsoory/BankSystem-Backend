
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RepositoryContracts;
using ServiceContract;
using System.Reflection;
/*
namespace Service.Cache
{
    public class CacheWarmupService : BackgroundService
    {
        private readonly IEntityCache _cache;
        private readonly IServiceProvider _serviceProvider;

        // 🔒 Semaphore ensures only one refresh runs at a time to prevent RAM spikes
        private static readonly SemaphoreSlim _gateKeeper = new(1, 1);

        public CacheWarmupService(IEntityCache cache, IServiceProvider serviceProvider)
        {
            _cache = cache;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initial startup targets
            var startupTargets = new List<(Type EntityType, string TableName)>
            {
               // (typeof(Person), "People"),
                //(typeof(Client), "Clients"),
                //(typeof(Account), "Accounts")
            };

            // Run the initial warmup
            await RefreshCacheAsync(startupTargets, stoppingToken);

            // Signal the "Shield" (Bitmask Filter) that it can start filtering requests
            _cache.MarkAsReady();
        }

        /// <summary>
        /// Orchestrates the streaming of IDs from DB to Bitmask Cache.
        /// </summary>
        public async Task RefreshCacheAsync(List<(Type EntityType, string TableName)>? targets = null, CancellationToken ct = default)
        {
            // 1. Entrance Control: If a refresh is already running, skip this request
            // to avoid allocating multiple DbContexts and crashing the 86MB RAM.
            if (!await _gateKeeper.WaitAsync(0, ct)) return;

            try
            {
                var targetsToProcess = targets ?? new List<(Type EntityType, string TableName)>
                {
                  //  (typeof(Account), "Accounts")
                };

                // 2. Create a Scope: This is the ONLY way for a Singleton service
                // to access a Scoped Repository safely.
                using var scope = _serviceProvider.CreateScope();

                // 3. Resolve the Repository (Service layer does NOT see the DbContext)
              

                foreach (var target in targetsToProcess)
                {
                    ct.ThrowIfCancellationRequested();

                    // 4. Fetch the Snapshot Size (O(1) operation)
                    // int maxId = await warmupRepo.GetMaxIdAsync(target.TableName, ct);
                    int maxId = 123;
                    // 5. Open the Pipe (IAsyncEnumerable)
                    // No data is fetched yet; this is just the "Instructions"
                    // var idStream = warmupRepo.GetActiveIdsStreamAsync(target.TableName, ct);
                    var idStream = false;
                    // 6. Reflection: Dynamically call _cache.Initialize<T>()
                    var method = _cache.GetType()
                        .GetMethod("Initialize")?
                        .MakeGenericMethod(target.EntityType);

                    if (method != null)
                    {
                        // 🚀 AWAIT THE STREAM: 
                        // This moves the IDs one-by-one into the BitArray.
                        // We cast to Task because Initialize is an 'async Task' method.
                        var task = (Task)method.Invoke(_cache, new object[] { maxId, idStream })!;
                        await task;
                    }

                 
                }
            }
            catch (Exception ex)
            {
                // In a real bank, log this to a file or monitoring system
                Console.WriteLine($"[Cache Error] Warmup failed: {ex.Message}");
            }
            finally
            {
                // 🔓 Always release the gate so the next refresh can run later
                _gateKeeper.Release();
            }
        }
    }
}*/