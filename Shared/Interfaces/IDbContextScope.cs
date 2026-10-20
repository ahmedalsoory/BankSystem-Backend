using System.Data;

namespace Shared.Interfaces
{
    public interface IDbContextScope : IDbConnectionProvider , IAsyncDisposable
    {

        ValueTask OpenConnectionAsync();
        void CloseConnection();
        void Commit();
        void Rollback();
    }
}
