using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IDbConnectionProvider
    {
        ValueTask<IDbConnection> GetConnectionAsync();
        ValueTask BeginTransactionAsync();
        IDbTransaction? CurrentTransaction { get; }
        Task<T?> ExecuteTransientAsync<T>(Func<IDbConnection, Task<T>> operation);
        CancellationToken CancellationToken { get; }
    }
}
