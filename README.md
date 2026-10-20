# BankSystem Backend

A banking-oriented ASP.NET Core Web API built with .NET 6, SQL Server, Dapper, FluentValidation, and a layered service/repository architecture.

The project is structured as a multi-project solution with explicit contracts between API, services, repositories, DTOs, and shared infrastructure. It also contains an integration/load-testing suite built around `WebApplicationFactory`, xUnit, FluentAssertions, and NBomber.

> **Project status**
>
> This project is actively developed. The repository contains the backend implementation together with integration and load-testing infrastructure. Some infrastructure and production-hardening areas are still evolving.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Solution Structure](#solution-structure)
- [Application Flow](#application-flow)
- [Core Architectural Decisions](#core-architectural-decisions)
  - [Layered Architecture](#layered-architecture)
  - [Repository and Service Contracts](#repository-and-service-contracts)
  - [Scoped Database Context and Transactions](#scoped-database-context-and-transactions)
  - [Transaction-Aware Orchestration](#transaction-aware-orchestration)
  - [Audit Tracking](#audit-tracking)
  - [Validation Pipeline](#validation-pipeline)
  - [Image Lifecycle Management](#image-lifecycle-management)
  - [Resilience and Retries](#resilience-and-retries)
  - [Performance Monitoring](#performance-monitoring)
  - [Exception Handling](#exception-handling)
  - [Database Access and Query Organization](#database-access-and-query-organization)
  - [Advanced Performance & Database Optimization](#advanced-performance--database-optimization)
  - [Concurrency Strategy & Trade-offs](#concurrency-strategy--trade-offs)
  - [Database Setup & Structure](#database-setup--structure)
- [Banking Domains](#banking-domains)
- [Testing](#testing)
- [Testing Strategy](#testing-strategy)
- [Load Testing](#load-testing)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Running the API](#running-the-api)
- [Running Tests](#running-tests)
- [Running Load Tests](#running-load-tests)
- [Database Notes](#database-notes)
- [Project Structure](#project-structure)
- [Engineering Focus](#engineering-focus)
- [Current Limitations and Future Work](#current-limitations-and-future-work)
- [License](#license)

---

## Overview

The BankSystem backend models a set of banking workflows including:

- Clients and persons
- Accounts
- Account applications
- Account-opening applications
- Account-opening details and workflow steps
- Transactions and account balances
- Application types
- Cards and card applications
- Checkbooks and checkbook applications
- Onboarding configuration
- Dashboard aggregation
- Validation and transactional read/write operations
- CSV export functionality

The project is intentionally separated into application, service, repository, contract, DTO, and shared infrastructure projects rather than placing the entire API in a single assembly.

---

# Architecture

At a high level, requests move through the following layers:

```text
┌───────────────────────────────┐
│        HTTP Client            │
└───────────────┬───────────────┘
                │
                ▼
┌───────────────────────────────┐
│       ASP.NET Core API        │
│ Controllers / Middleware      │
│ Filters / Validation          │
└───────────────┬───────────────┘
                │
                ▼
┌───────────────────────────────┐
│      Service Contracts        │
│   Application / Domain API    │
└───────────────┬───────────────┘
                │
                ▼
┌───────────────────────────────┐
│          Services             │
│ Business Logic / Orchestrators│
└───────────────┬───────────────┘
                │
                ▼
┌───────────────────────────────┐
│    Repository Contracts       │
└───────────────┬───────────────┘
                │
                ▼
┌───────────────────────────────┐
│         Repositories          │
│ Dapper + SQL Server           │
└───────────────┬───────────────┘
                │
                ▼
┌───────────────────────────────┐
│          SQL Server           │
└───────────────────────────────┘
```

Shared infrastructure provides cross-cutting concerns such as:

- Database connection/transaction scope
- Audit tracking
- Image handling
- Shared operation-result types
- Request context
- Common exceptions and infrastructure interfaces

---

# Solution Structure

The solution is divided into the following projects:

| Project | Responsibility |
|---|---|
| `API` | ASP.NET Core Web API, controllers, middleware, filters and application startup |
| `DTOs` | Request/response DTOs and data contracts |
| `RepositoryContracts` | Repository interfaces and persistence contracts |
| `Repositories` | Dapper/SQL Server data-access implementations |
| `ServiceContract` | Service interfaces and application-level contracts |
| `Service` | Business logic, validation, services and orchestrators |
| `Shared` | Cross-cutting infrastructure and shared abstractions |
| `Tests` | Integration tests, test builders, workflow tests and NBomber load tests |

The solution targets **.NET 6**.

---

# Application Flow

A typical write request follows this general sequence:

```text
HTTP Request
    │
    ▼
ASP.NET Core Middleware
    │
    ▼
MVC Filter Pipeline
    │
    ├── Performance measurement
    │
    ├── FluentValidation
    │
    └── Database/transaction scope
    │
    ▼
Controller
    │
    ▼
Service / Orchestrator
    │
    ▼
Repository
    │
    ▼
Dapper
    │
    ▼
SQL Server
```

For successful transactional operations, the database transaction is committed after the endpoint completes successfully.

For failed operations or unhandled exceptions, the transaction is rolled back.

---

# Core Architectural Decisions

## Layered Architecture

The backend deliberately separates responsibilities:

```text
API
 │
 ├── Controllers
 ├── Middleware
 └── Filters
      │
      ▼
ServiceContract
      │
      ▼
Service
      │
      ▼
RepositoryContracts
      │
      ▼
Repositories
      │
      ▼
SQL Server
```

This keeps HTTP concerns away from business logic and keeps persistence details behind repository contracts.

---

## Repository and Service Contracts

Repositories and services commonly expose multiple interfaces depending on the responsibility they provide.

For example, a repository can implement separate read/write/transaction-oriented contracts while still being registered as a single scoped concrete instance.

The dependency-injection configuration explicitly forwards interfaces to the same scoped implementation:

```csharp
services.AddScoped<ClientRepository>();

services.AddScoped<IClientReadRepository>(
    sp => sp.GetRequiredService<ClientRepository>());

services.AddScoped<IClientWriteRepository>(
    sp => sp.GetRequiredService<ClientRepository>());
```

This is important for operations that need to share the same scoped database context and transaction.

The same pattern is used throughout several major banking domains.

---

## Scoped Database Context and Transactions

The project uses a custom `DbContextScope` rather than an ORM-specific unit of work.

The scope is responsible for:

- Creating the SQL Server connection
- Opening the connection using the current request cancellation token
- Maintaining the current transaction
- Exposing the active transaction to repositories
- Committing transactions
- Rolling back transactions
- Closing and disposing the connection
- Flushing pending audit records

Repositories obtain their connection through the shared abstraction instead of independently creating database connections.

Conceptually:

```text
HTTP Request / DI Scope
        │
        ▼
   DbContextScope
        │
        ├── SqlConnection
        │
        └── SqlTransaction
              │
       ┌──────┼────────┐
       ▼      ▼        ▼
   Client  Account  Transaction
 Repository Repository Repository
```

This allows several repository/service operations belonging to one business workflow to participate in the same database transaction.

---

## Transaction-Aware Orchestration

The project contains higher-level orchestrators for workflows that span multiple services.

One example is account opening.

The account-opening workflow can perform operations such as:

1. Verify account-opening details.
2. Complete the corresponding workflow step.
3. Determine whether the workflow is complete.
4. Retrieve the application data transactionally.
5. Mark the application as completed.
6. Create the account.
7. Create the initial deposit transaction.

The intent is that these related database changes belong to one business operation and therefore should succeed or fail together.

Simplified:

```text
Account Opening Workflow
        │
        ├── Verify details
        ├── Complete workflow step
        ├── Check workflow completion
        ├── Complete application
        ├── Create account
        └── Create initial deposit
                │
                ▼
        Shared DB Transaction
```

---

## Audit Tracking

Mutating operations can register audit information through `IAuditTracker`.

Audit entries contain information such as:

- Table name
- Record ID
- Operation type
- User ID

The entries are held during the request and flushed to `AuditLogs` as part of transaction commit.

This gives the audit operation the same transactional boundary as the associated database changes.

If the business transaction is rolled back, pending audit entries are cleared rather than being persisted independently.

---

## Validation Pipeline

The API uses FluentValidation together with a custom MVC filter:

`AutomaticFluentValidationFilter`

The filter:

1. Inspects action arguments.
2. Detects DTOs implementing the validation marker contract.
3. Resolves the corresponding `IValidator<T>` from dependency injection.
4. Runs asynchronous validation.
5. Short-circuits the request with a `400 Bad Request` when validation fails.

The global filter registration places validation before the database connection/transaction filter:

```text
PerformanceFilter
       ↓
AutomaticFluentValidationFilter
       ↓
GlobalConnectionFilter
       ↓
Controller Action
```

This allows invalid requests to be rejected before database work is initialized.

---

## Image Lifecycle Management

The project includes an image subsystem built around:

- `IImageService`
- `IImageHandler`
- `IImageTracker`
- `ImageCommitActionFilter`

Supported image extensions include:

- `.jpg`
- `.jpeg`
- `.png`
- `.webp`

The current maximum uploaded file size is **2 MB**.

Images are loaded and re-encoded through ImageSharp before being written to the application's `wwwroot/images` hierarchy.

The image tracker distinguishes between:

```text
New Images
    │
    └── Delete when the database operation fails

Old Images
    │
    └── Delete after the database operation succeeds
```

This prevents a common consistency problem where a database update fails but an uploaded replacement file remains on disk, or where an old file is deleted before the database transaction is safely committed.

The image subsystem also attempts to reject invalid or corrupted image payloads through ImageSharp decoding.

---

## Resilience and Retries

The project uses Polly for selected transient SQL failures.

The database infrastructure recognizes transient SQL errors including:

- `1205` — deadlock
- `-2` — SQL timeout
- `40613` — database unavailable/busy condition
- `40197` — SQL service processing/request error

The API's global database filter also defines retry handling for selected deadlock and timeout conditions when a transaction is active.

Retrying database operations requires particular care around transaction state. The implementation therefore resets/disposes the current transaction and connection before another attempt.

Retries are intended for transient infrastructure failures rather than ordinary business validation errors.

---

## Performance Monitoring

The API contains a `PerformanceFilter` that measures endpoint execution time.

Every request is logged with:

- HTTP method
- Request path
- Execution duration
- Timestamp

A threshold of **1000 ms** is used for slow-endpoint detection outside development/debugging environments.

The project also contains a performance alert service using `HttpClient`, with Telegram-based alerting for slow endpoints and unexpected server exceptions.

---

## Exception Handling

`ExceptionMiddleware` provides centralized exception handling.


The error response includes information such as:

```json
{
  "StatusCode": 500,
  "ErrorCode": "SERVER_ERROR",
  "Message": "A server error occurred.",
  "Errors": null,
  "TraceId": "..."
}
```

The trace ID is useful for correlating client-visible failures with server-side logs.

Unexpected server errors are also eligible for asynchronous Telegram alerts.

---

## Database Access and Query Organization

The project uses **Dapper** with `Microsoft.Data.SqlClient` rather than Entity Framework Core.

SQL operations are organized under the repository layer.

The solution also contains a centralized `Repositories.Queries.Query` container that groups query definitions by domain:

```text
Query
├── Account
├── AccountApplication
├── AccountOpeningApplication
├── AccountOpeningDetail
├── AccountWorkflow
├── ApplicationType
├── Client
├── Dashboard
├── Person
├── Transaction
├── Card
├── CardApplication
├── Checkbook
└── CheckbookApplication
```

This keeps repository classes from becoming a collection of scattered SQL string literals.

The database layer also includes specialized paging implementations and stored-procedure calls for larger result sets.

---

## Advanced Performance & Database Optimization

To handle heavy database workloads and performance bottlenecks efficiently, the backend implements the following enterprise-level optimizations:

* **Deferred Join & Seek-Based Pagination:** Utilizes CTEs to fetch primary keys first, eliminating expensive `Key Lookups` and maintaining constant response times even during deep pagination.
* **Cached Count & Metadata Optimization:** Bypasses costly `COUNT(*)` execution plan costs by reusing client-side cached total counts or leveraging ultra-fast metadata partition statistics (`sys.dm_db_partition_stats`) when no custom filters are applied.
* **Dynamic Parameterized Execution:** Secures all dynamic queries against SQL injection using `sp_executesql` while ensuring optimal query execution plans.
* **Optimistic Concurrency & Security:** Implements `RowVersion` (`timestamp`) for safe concurrent updates and strict hashing for sensitive data like passwords, card numbers, and CVVs.

---

## Concurrency Strategy & Trade-offs

In financial and banking systems, managing concurrent requests and race conditions is critical to prevent data corruption or violating business constraints. The backend employs clear strategies tailored to specific operational requirements:

- **Unique Constraints as a First Line of Defense:** 
  Database unique constraints are heavily relied upon for foundational checks (like validating unique emails or phone numbers) to prevent duplicates at the schema level while avoiding unnecessary locking.
- **Pessimistic Range Locking:** 
  For business rules enforcing strict limits (such as restricting a client to a maximum of 5 accounts), queries utilize `UPDLOCK` combined with `HOLDLOCK` to ensure concurrent requests cannot bypass account thresholds during simultaneous creations.
- **Targeted Row Locking for Critical Operations:** 
  In financial transactions (like purchases, deposits, and tax deductions), rejecting user requests due to concurrency conflicts is unacceptable. Precise row locking (`UPDLOCK`) secures balances within transactional boundaries without locking entire tables.
- **Optimistic Concurrency for Low-Risk Updates:** 
  For non-critical mutations like profile updates, `RowVersion` (`timestamp`) tracking is used. Conflicting updates can safely prompt the client to refresh, causing no financial friction.
- **Transient Failure Resilience:** 
  Integrated with Polly, the data-access layer automatically handles temporary deadlocks (SQL error `1205`) by safely retrying operations.

- **Atomic Conditional Updates (State-Based Optimistic Concurrency):** To prevent race conditions during high-throughput status updates (e.g., approving or rejecting account applications) without enforcing heavy pessimistic locks, updates execute atomic conditional state transitions directly at the SQL level:
**Conditional Enforcement:** The `WHERE` clause strictly mandates the valid source state (`Status IN (0, 1)`) and allowed target transition (`@NewStatus IN (2, 3)`).
  * **Race Condition Mitigation:** If two concurrent requests target the same record, only the first transaction modifies the row; the second fails gracefully with zero rows affected.

sql
`UPDATE Apps.AccountOpeningDetails
SET Status = @NewStatus,
    RejectionReason = @RejectionReason,
    LastModifiedDate = GETDATE()
WHERE DetailId = @DetailId
  AND Status IN (0, 1)  
  AND @NewStatus IN (2, 3);`

---

## Database Setup & Structure

The repository includes a complete SQL schema script structured into specialized enterprise schemas (`Apps`, `Banking`, `Core`, `Loans`, `Ref`, `System`).

1. Locate the master initialization script in `dbScript/001_InitialSchema.sql`.
2. Run the script against your local **SQL Server** instance to set up the `BankSystemDB` database and optimized stored procedures.

---

# Banking Domains

The current backend contains functionality around several areas:

### Client / Person

- Create clients
- Update client information
- Retrieve clients
- Retrieve client details
- Client paging
- Person-level validation

### Accounts

- Create accounts
- Retrieve accounts
- Retrieve by account number
- Retrieve by client
- Update account status
- Account locking for transaction workflows

### Transactions

- Deposit
- Withdrawal
- Balance updates
- Transaction logging
- Transaction-oriented reads

### Account Applications

- Create applications
- Retrieve applications
- Update application status
- Application type lookup
- Paged application queries

### Account Opening

- Account-opening applications
- Account-opening details
- Workflow steps
- Status verification
- Application completion
- Account creation
- Initial deposit workflow

### Cards

- Card applications
- First-time card issuance
- Replacement applications
- Renewal applications
- Card status
- Card detail retrieval

### Checkbooks

- New checkbook applications
- Replacement applications
- Renewal applications
- Checkbook data retrieval

### Onboarding

- Onboarding application types
- Onboarding type steps
- Application workflow steps

### Dashboard

- System counters
- Historical data
- Background aggregation

### Export

- CSV export functionality

---

# Testing

Testing is included in the same solution and is not limited to isolated unit tests.

The test project uses:

- xUnit
- FluentAssertions
- Microsoft.AspNetCore.Mvc.Testing
- AutoFixture
- NBomber
- Coverlet collector

The integration-test infrastructure uses `WebApplicationFactory<Program>` and shared test fixtures to exercise the real application composition.
---

## Testing Strategy

For this repository, I explicitly prioritized **End-to-End Integration Testing** and **Performance Load Testing** over isolated unit tests with heavy mocking.

* **Real-World Confidence:** Integration tests exercise real HTTP pipelines, route mapping, request validation, and database transactions working together end-to-end.
* **Pragmatic ROI for Solo Development:** During rapid solo development, implementation details and code structures evolve frequently. Heavy unit testing with strict mocks creates tight coupling, requiring constant test rewrites without adding significant value. Integration tests validate overall API behavior and contract fulfillment without slowing down rapid refactoring.
* **Performance & Concurrency Validation:** Load testing ensures system throughput, query execution, and database concurrency hold up under stress.

> **Team Environment Context:** When working within an engineering team where architectural designs, response contracts, and specifications are explicitly defined by architects, I write unit tests to strictly validate method-level contracts, handle edge cases, and ensure isolated changes do not break shared modules.
> 
---
## Integration Tests

The repository contains integration tests for areas including:

- Clients
- Accounts
- Account applications
- Account-opening workflows
- Onboarding
- Service/repository interactions

The test suite also includes custom builders such as:

```text
ClientBuilder
PersonBuilder
AccountApplicationBuilder
AccountOpeningApplicationBuilder
VerifiedStatusRequestBuilder
```

These builders make complex test scenarios easier to construct and allow tests to describe the scenario rather than repeatedly rebuilding DTOs manually.

---

# Load Testing

NBomber is used for application-level load testing.

The repository includes load tests for scenarios such as:

### Client Creation

A client creation test exercises:

```text
POST /api/Client/create
```

with sustained request injection.

### Client Creation with Images

A separate scenario includes a real in-memory PNG payload.

The test uses multiple phases:

```text
Warm-up
   ↓
50 RPS ramp

Sustained
   ↓
100 RPS

Stress
   ↓
200 RPS
```

The test reports metrics including:

- Successful request count
- Requests per second
- Mean latency
- P95 latency
- P99 latency
- Failed requests

### Client Updates

The update load test exercises:

```text
GET /api/Client/{id}
        ↓
Build update payload
        ↓
PUT /api/Client/update
```

The update request includes the relevant row-version values, allowing concurrency behavior to be exercised under load.

### Account Opening Workflow

The account-opening workflow has a dedicated NBomber scenario.

The current load test injects:

```text
200 requests/second
for 30 seconds
```

and records:

- Successful requests
- RPS
- Mean latency
- P95 latency
- Failures

> Load-test numbers depend on the machine, SQL Server instance, database state, network, and runtime environment. The repository defines the scenarios and measurement process; benchmark results should be interpreted together with the environment in which they were executed.

---

# Technology Stack

## Runtime

- .NET 6
- ASP.NET Core Web API
- C#

## Database

- Microsoft SQL Server
- `Microsoft.Data.SqlClient`
- Dapper

## Application Architecture

- Layered architecture
- Repository pattern
- Service layer
- Service/repository contracts
- Application orchestrators
- Dependency injection
- Scoped transaction context

## Validation

- FluentValidation

## Resilience

- Polly

## Image Processing

- SixLabors ImageSharp

## Logging / Monitoring

- Serilog
- SQL Server Serilog sink
- Telegram performance/error alerts

## API Documentation

- Swashbuckle / Swagger

## Testing

- xUnit
- FluentAssertions
- AutoFixture
- ASP.NET Core `WebApplicationFactory`
- NBomber
- Coverlet

---

# Prerequisites

To build and run the project locally, you should have:

- .NET 6 SDK
- SQL Server / SQL Server LocalDB
- A configured `BankSystemDB` database
- A configured `BankLogsDB` database if SQL logging is enabled
- Git

The repository currently targets:

```text
net6.0
```

---

# Configuration

The API reads the database configuration from:

```text
API/appsettings.json
```

The current development configuration uses SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BankSystemDB;Max Pool Size=100;Connect Timeout=60;Integrated Security=True;Encrypt=False;Trust Server Certificate=False;",
    "LogDatabase": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BankLogsDB;Max Pool Size=5;Integrated Security=True;Connect Timeout=5;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
  }
}
```

For your own environment, change these values to match your SQL Server configuration.

Do not commit real production credentials or secrets to source control.

The project also defines a .NET User Secrets ID for development configuration.

---

# Running the API

From the solution directory:

```bash
dotnet restore
dotnet build
```

Then run the API project:

```bash
dotnet run --project API/API.csproj
```

In development, Swagger is enabled by the application pipeline.

The API also serves static files because uploaded images are stored under the application's `wwwroot/images` hierarchy.

---

# Running Tests

Run the complete test project with:

```bash
dotnet test Tests/Tests.csproj
```

Or from the solution root:

```bash
dotnet test
```

The integration tests require a working database configuration because they exercise real application components and persistence behavior.

---

# Running Load Tests

The load tests are implemented as xUnit tests using NBomber.

Examples include:

```text
Client creation
Client creation with image
Client update
Account opening workflow
```

They can be executed through the normal .NET test runner:

```bash
dotnet test Tests/Tests.csproj
```

Because load tests intentionally generate database and application traffic, they should normally be executed against a dedicated test environment rather than a production database.

---

# Database Notes

The backend expects a SQL Server database named:

```text
BankSystemDB
```

The repository contains the application-side database access code, but the database schema itself is not represented as a complete SQL migration project in this repository snapshot.

That means the following should be treated as external prerequisites when setting up the application:

- Database schema
- Tables
- Indexes
- Constraints
- Stored procedures used by repositories
- Required seed/reference data

Some repository methods call stored procedures for operations such as paged queries, so the corresponding database objects must exist in the configured database.

---

# Project Structure

A simplified structure is:

```text
BankSystem.sln
│
├── API/
│   ├── Controllers/
│   ├── Attributes/
│   ├── Filter/
│   ├── Middleware/
│   ├── Extensions/
│   ├── Program.cs
│   └── appsettings.json
│
├── DTOs/
│   ├── Account/
│   ├── AccountApplications/
│   ├── AccountOpening/
│   ├── Client/
│   ├── Person/
│   ├── Transaction/
│   ├── Card/
│   └── ...
│
├── RepositoryContracts/
│   ├── Account/
│   ├── AccountApplications/
│   ├── AccountOpening/
│   ├── Client/
│   ├── Person/
│   ├── Transaction/
│   └── ...
│
├── Repositories/
│   ├── Apps/
│   ├── Banking/
│   ├── Core/
│   ├── Validations/
│   ├── Queries/
│   └── Extensions/
│
├── ServiceContract/
│   ├── Account/
│   ├── AccountApplications/
│   ├── AccountOpening/
│   ├── Client/
│   ├── Transaction/
│   └── ...
│
├── Service/
│   ├── Core/
│   ├── Apps/
│   ├── Banking/
│   ├── Validation/
│   ├── Orchestrators/
│   └── Extension Method/
│
├── Shared/
│   ├── Image/
│   ├── Interfaces/
│   ├── Exceptions/
│   ├── Extensions/
│   ├── Enums/
│   └── DbContextScope.cs
│
└── Tests/
    ├── Account/
    ├── AccountApplications/
    ├── AccountOpeningOrchestrator/
    ├── Client/
    ├── TestBuilders/
    ├── Helper/
    └── Globle/
```

---

# Engineering Focus

A major goal of the project is not simply implementing CRUD endpoints, but exploring the behavior of a backend system under transactional and concurrent workloads.

The implementation therefore pays particular attention to:

### Transaction boundaries

Keeping related database changes inside a shared transaction.

### Concurrency

Using SQL transactions, account locking, row-version values and concurrency-aware application behavior.

### Performance

Using Dapper, explicit SQL, paging strategies, connection pooling and load testing to investigate performance characteristics.

### Database efficiency

The repository contains specialized query paths for paged data and stored-procedure based operations rather than relying exclusively on generic ORM-generated SQL.

### Observability

Performance logging, structured exception information, trace IDs and external alerts are included to make failures and slow requests easier to investigate.

### Failure consistency

Database transactions and image tracking are coordinated so that a failed business operation does not intentionally leave newly created files behind.

### Integration testing

The project tests application composition and persistence behavior instead of relying only on mocked unit tests.

---

# Current Limitations and Future Work

The project is still under development. Areas that can continue to evolve include:

- Expanding integration-test coverage across all domains
- Increasing unit-test coverage for complex orchestration logic
- Improving test determinism for concurrent execution
- Further production hardening of background alert delivery
- More comprehensive observability and metrics
- CI/CD automation
- Containerized deployment
- Database migration/versioning infrastructure
- Further security hardening
- More extensive authorization/authentication integration
- Additional performance profiling under production-like database sizes
- More comprehensive failure/recovery testing

These are development directions rather than claims that the current implementation already provides all of them.

---

# License

No explicit license file is included in this repository snapshot.

If this project is intended for public reuse, add an appropriate `LICENSE` file and update this section accordingly.
