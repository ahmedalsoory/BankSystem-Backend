using System.Data;

namespace Shared.Interfaces
{
    public interface IDbContextScope : IDbConnectionProvider , IAsyncDisposable
    {

        ValueTask OpenConnectionAsync();
        void CloseConnection();
        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation);
        Task ExecuteWithRetryAsync(Func<Task> operation);
        void Commit();
        void Rollback();
    }
}
