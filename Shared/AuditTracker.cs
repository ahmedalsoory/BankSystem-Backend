using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Shared
{
    public class AuditLogEntry
    {
        public string TableName { get; set; } = string.Empty;
        public string RecordId { get; set; } = string.Empty;
        public string OperationType { get; set; } = string.Empty;
        public string? UserId { get; set; }
    }
    public interface IAuditTracker
    {
        void AddEntry(string tableName, string recordId, string operationType, string? userId);
        IReadOnlyCollection<AuditLogEntry> GetEntries();
        void Clear();
    }

    public class AuditTracker : IAuditTracker
    {
        private readonly List<AuditLogEntry> _entries = new();

        public void AddEntry(string tableName, string recordId, string operationType, string? userId)
        {
            _entries.Add(new AuditLogEntry
            {
                TableName = tableName,
                RecordId = recordId,
                OperationType = operationType,
                UserId = userId
            });
        }

        public IReadOnlyCollection<AuditLogEntry> GetEntries() => _entries.AsReadOnly();

        public void Clear() => _entries.Clear();
    }
}
