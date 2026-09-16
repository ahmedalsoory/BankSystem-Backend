using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.CSVExportService
{
    public interface ICSVExportService
    {
        Task<string> ExportAndCompressToZipAsync<T>(
             IAsyncEnumerable<T> dataStream,
             CancellationToken cancellationToken = default);

    }
}
