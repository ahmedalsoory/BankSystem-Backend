using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.ExportRepository
{
    public interface IExportRepository
    {
        IAsyncEnumerable<T> StreamQueryAsync<T>(string sqlQuery, object? parameters = null,
            CancellationToken cancellationToken = default);

    }
}

