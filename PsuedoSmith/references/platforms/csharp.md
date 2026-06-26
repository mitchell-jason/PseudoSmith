# Platform Reference: C#

## Default Version

Default target: .NET 8 / C# 12 for new code unless the blueprint specifies .NET Framework,
.NET 6 LTS, or another target.

If broad enterprise compatibility is implied, prefer .NET 6 / C# 10 and report the choice.

## Namespaces and Files

- `NAMESPACE` becomes a C# namespace declaration.
- Do not impose directory layout from namespace unless the blueprint or project policy says so.
- Use file-scoped namespaces only when target version supports them and style permits.

## Data Types

- `STRING` -> `string`
- `INT` -> `int`
- `LONG` -> `long`
- `DECIMAL` -> `decimal`
- `BOOLEAN` -> `bool`
- `LIST` -> `List<T>`
- `COLLECTION` -> `HashSet<T>`
- `DICTIONARY` -> `Dictionary<TKey,TValue>`
- `DATETIME` -> `DateTime` or `DateTimeOffset` when timezone semantics matter.
- `CURRENCY` -> `decimal`; never `float`/`double` for monetary values.
- `DELEGATE` -> a `delegate` type, or `Func<...>`/`Action<...>` when a built-in signature fits.

Enable nullable reference types for modern .NET unless the blueprint/style guide says otherwise.

## GUI

GUI anchors require `TARGET_UI_FRAMEWORK`.

Rules:

- `WinForms` and `WPF` are Windows-only. Do not generate them for Linux/macOS.
- `MAUI` and `Avalonia` are cross-platform but require explicit declaration.
- Do not choose a UI framework silently.

## Database

C# has provider-specific database libraries. `DATABASE_PROVIDER` is required for database code.

Require `DATABASE_DRIVER` or `USES` when the provider is not available in the selected target
without extra packages.

Common explicit drivers:

- SQL Server: `Microsoft.Data.SqlClient` or `System.Data.SqlClient`
- SQLite: `Microsoft.Data.Sqlite` or `System.Data.SQLite`
- PostgreSQL: `Npgsql`
- MySQL/MariaDB: `MySqlConnector`
- Oracle: `Oracle.ManagedDataAccess`
- ODBC: `System.Data.Odbc`

Do not add Entity Framework, Dapper, or an ORM unless requested or declared.

## Async and Concurrency

- `ASYNC`/`AWAIT` -> `async Task`, `Task<T>`, and `await`.
- Locks -> `lock`, `SemaphoreSlim`, or `Mutex` according to blueprint intent.
- For I/O-bound async, use async APIs only when available and requested or implied by
  `CONCURRENCY_MODEL`/`DATABASE_ACCESS`.

## Platform Compatibility

- .NET Framework 4.x is Windows-only.
- Win32 API calls require `System.Runtime.InteropServices` and platform guards.
- Do not generate Windows-only APIs for Linux/macOS targets.
- `System.Text.Json` is modern .NET. For older .NET Framework, require declared dependency
  before using `Newtonsoft.Json`.

## Version Fallbacks

- `record` requires C# 9+.
- file-scoped namespaces and global usings require C# 10+.
- `required` members require C# 11+.
- If unavailable, generate class/constructor equivalents and report the fallback.
