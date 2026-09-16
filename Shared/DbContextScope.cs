using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Polly;

namespace Shared
{
    public class DbContextScope : IDbContextScope
    {
        private readonly string _connectionString;
        private readonly IAuditTracker _auditTracker;
        private  IDbConnection? _connection;
        private IDbTransaction? _transaction;
        private readonly IHttpContextAccessor _httpContextAccessor; // 2. Declare the accessor

        public DbContextScope(IConfiguration config, IHttpContextAccessor httpContextAccessor,
            IAuditTracker auditTracker)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(config), "Connection string 'DefaultConnection' is missing!");
            _auditTracker = auditTracker ?? throw new ArgumentNullException(nameof(auditTracker));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public CancellationToken CurrentRequestToken =>
                _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        public async ValueTask<IDbConnection> GetConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State == ConnectionState.Closed)
            {
                if (_connection is SqlConnection sqlConn)
                {
                    await sqlConn.OpenAsync(CurrentRequestToken);
                }
                else
                {
                    _connection.Open();
                }
            }

            return _connection;
        }

        // Exposes the current transaction to repositories
        public IDbTransaction? CurrentTransaction => _transaction;

        public CancellationToken CancellationToken =>
            _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        // 🚀 IDEMPOTENT & REUSABLE: Safe against nested or parallel middleware/repository calls
        public async ValueTask BeginTransactionAsync()
        {
            var connection = await GetConnectionAsync();

            if (_transaction != null)
            {
                return; // Early return to transparently reuse the outer transaction pipe
            }

            _transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                // 📝 AUDIT INTEGRATION: Flush any pending audit entries before committing the transaction
                var entries = _auditTracker.GetEntries();
                if (entries.Any() && _connection != null)
                {
                    const string auditSql = @"
                        INSERT INTO AuditLogs (TableName, RecordId, OperationType, UserId, CreatedAt) 
                        VALUES (@TableName, @RecordId, @OperationType, @UserId, GETUTCDATE())";

                    _connection.Execute(auditSql, entries, _transaction);
                    _auditTracker.Clear();
                }

                _transaction?.Commit();
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public void Rollback()
        {
            if (_transaction == null) return;

            try
            {
                // 🚀 THE ZOMBIE SHIELD: Check if the underlying connection is still alive and attached.
                if (_transaction.Connection != null && _transaction.Connection.State == ConnectionState.Open)
                {
                    _transaction.Rollback();
                }
            }
            catch (InvalidOperationException)
            {
                // Fail-safe catch block: If the transaction became a zombie, swallow safely.
            }
            finally
            {
                DisposeTransaction();
                // Clear out uncommitted audit entries so they don't leak into subsequent requests
                _auditTracker.Clear();
            }
        }

        public async ValueTask OpenConnectionAsync()
        {
            await GetConnectionAsync();
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        private void DisposeTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
        }

        public async ValueTask DisposeAsync()
        {
            DisposeTransaction();

            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }

                if (_connection is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    _connection.Dispose();
                }
                _connection = null;
            }
        }

        public async Task<T?> ExecuteTransientAsync<T>(Func<IDbConnection, Task<T>> operation)
        {
            var retryPolicy = Policy
                .Handle<SqlException>(ex => IsTransient(ex))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            return await retryPolicy.ExecuteAsync(async () =>
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await operation(connection);
                }
            });
        }

        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
        {
            var retryPolicy = Policy
                .Handle<SqlException>(ex => IsTransient(ex))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(50 * Math.Pow(2, retryAttempt - 1)));

            return await retryPolicy.ExecuteAsync(async () =>
            {
                DisposeTransaction();
                CloseConnection();

                return await operation();
            });
        }

        public async Task ExecuteWithRetryAsync(Func<Task> operation)
        {
            var retryPolicy = Policy
                .Handle<SqlException>(ex => IsTransient(ex))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(50 * Math.Pow(2, retryAttempt - 1)));

            await retryPolicy.ExecuteAsync(async () =>
            {
                DisposeTransaction();
                CloseConnection();

                await operation();
            });
        }

        private bool IsTransient(SqlException ex)
        {
            switch (ex.Number)
            {
                case 1205: // Deadlock
                case -2:   // Timeout
                case 40613: // Database busy
                case 40197: // Error processing request
                    return true;
                default:
                    return false;
            }
        }
    }
}