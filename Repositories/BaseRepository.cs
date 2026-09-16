using System;
using System.Data;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Shared.Interfaces;
using Shared;

public abstract class BaseRepository
{
    private readonly IDbConnectionProvider _dbScope;
    private readonly string _className;
    protected readonly IAuditTracker? _auditTracker;
    protected readonly Context _errorContext;
    protected readonly string? _connectionString;

    // Primary Constructor with optional audit tracker
    protected BaseRepository(IDbConnectionProvider dbScope, Context errorContext)
            : this(dbScope, errorContext, null, null)
    {
    }

    // 2. Constructor with Audit Tracker
    protected BaseRepository(IDbConnectionProvider dbScope, Context errorContext, IAuditTracker? auditTracker)
        : this(dbScope, errorContext, auditTracker, null)
    {
    }
    protected BaseRepository(IDbConnectionProvider dbScope, Context errorContext, IOptions<DbSettings>? connectionOptions)
      : this(dbScope, errorContext, null, connectionOptions)
    {
    }
    // 3. Fully Loaded Primary Constructor
    protected BaseRepository(
        IDbConnectionProvider dbScope,
        Context errorContext,
        IAuditTracker? auditTracker,
        IOptions<DbSettings>? connectionOptions)
    {
        _dbScope = dbScope ?? throw new ArgumentNullException(nameof(dbScope));
        _errorContext = errorContext ?? throw new ArgumentNullException(nameof(errorContext));
        _auditTracker = auditTracker;
        _connectionString = connectionOptions?.Value?.DefaultConnection;
        _className = this.GetType().Name;
    }
    // Helper to "Publish" the full name to your Context
    protected CancellationToken CancellationToken => _dbScope.CancellationToken;

    protected void SetAction([CallerMemberName] string methodName = "")
    {
        _errorContext.CurrentAction = $"{_className}.{methodName}";
    }

    // Safe helper: Uses null-conditional operator (?.) so it won't crash if auditing isn't injected
    protected void LogAudit(string tableName, string recordId, string operationType, string? userId)
    {
        _auditTracker?.AddEntry(tableName, recordId, operationType, userId);
    }

    protected ValueTask<IDbConnection> GetConnectionAsync()
    {
        return _dbScope.GetConnectionAsync();
    }

    protected async ValueTask BeginTransactionAsync() => await _dbScope.BeginTransactionAsync();

    protected IDbTransaction? CurrentTransaction => _dbScope.CurrentTransaction;

    protected async Task<T?> ExecuteTransientAsync<T>(Func<IDbConnection, Task<T>> operation)
    {
        return await _dbScope.ExecuteTransientAsync(operation);
    }
}