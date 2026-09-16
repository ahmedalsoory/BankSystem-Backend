using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceContract.CSVExportService;

namespace Service
{
    public class CSVExportService : ICSVExportService
    {
        public async Task<string> ExportAndCompressToZipAsync<T>(
            IAsyncEnumerable<T> dataStream,
            CancellationToken cancellationToken = default)
        {
            string tempCsvPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
            string tempZipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");

            try
            {
                // 1. Write raw CSV data row-by-row (keeps server memory flat)
                await using (var writer = new StreamWriter(tempCsvPath, append: false, Encoding.UTF8))
                {
                    PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    string headerLine = string.Join(",", properties.Select(p => p.Name));
                    await writer.WriteLineAsync(headerLine);

                    await foreach (var item in dataStream.WithCancellation(cancellationToken))
                    {
                        if (item == null) continue;

                        string[] values = properties.Select(p =>
                        {
                            var val = p.GetValue(item);
                            return FormatField(val?.ToString());
                        }).ToArray();

                        string rowLine = string.Join(",", values);
                        await writer.WriteLineAsync(rowLine.AsMemory(), cancellationToken);
                    }
                }

                // 2. Compress the massive CSV into a ZIP archive
                using (var zipFileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                {
                    using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
                    {
                        var csvEntry = archive.CreateEntry("Transactions.csv", CompressionLevel.Optimal);
                        using var entryStream = csvEntry.Open();
                        using var csvFileStream = new FileStream(tempCsvPath, FileMode.Open, FileAccess.Read);

                        await csvFileStream.CopyToAsync(entryStream, cancellationToken);
                    }
                }

                return tempZipPath;
            }
            finally
            {
                // 3. Always clean up the raw uncompressed CSV temp file
                if (File.Exists(tempCsvPath))
                {
                    File.Delete(tempCsvPath);
                }
            }
        }

        private string FormatField(string? field)
        {
            if (string.IsNullOrEmpty(field)) return string.Empty;
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}