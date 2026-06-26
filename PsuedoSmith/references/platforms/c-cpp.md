# Platform Reference: C and C++

## Defaults

C default: C11.
C++ default: C++17.

Use older/newer standards only when the blueprint or `TARGET_LANGUAGE_VERSION` specifies them.

## Files

- C/C++ may require header/source pairs.
- Generate headers only when needed for declarations shared across files.
- Use `#pragma once` for C++ headers unless style guide forbids it.
- File layout remains blueprint-owned.

## C Rules

- Map `CLASS` to `struct` plus functions taking a pointer to the struct.
- Map `INTERFACE` to a `struct` of function pointers (a vtable-style contract);
  implementers populate the function pointers. Record this realization in the report.
- Map `CURRENCY` to a fixed-point integer (e.g. cents in `int64_t`) or a declared
  decimal library; never use `float`/`double` for monetary values.
- Map `DELEGATE` to a function-pointer typedef.
- Pair every allocation with deallocation.
- Check `malloc` results.
- Prefer `snprintf`, bounded copies, and explicit buffer lengths.
- Use `<stdbool.h>` for booleans on modern C.
- Use fixed-width integer types when size matters.

## C++ Rules

- Map `INTERFACE` to an abstract base class with pure-virtual methods
  (`virtual ... = 0;`) and a virtual destructor. Implementers inherit and override.
- Map `CURRENCY` to a fixed-point integer or a declared decimal type; never use
  `float`/`double` for monetary values.
- Map `DELEGATE` to `std::function<...>` (or a function-pointer typedef when no capture is needed).
- Prefer RAII and smart pointers over raw `new`/`delete` on desktop/server targets.
- Use STL containers on non-embedded targets.
- Use `nullptr` rather than `NULL`.
- Use exceptions for I/O errors unless the blueprint or platform forbids them.
- Do not use C++20/23 features unless target version permits.

## Embedded RTOS

For `TARGET_PLATFORM = embedded_rtos`:

- avoid `malloc`, `free`, `new`, and `delete` unless explicitly requested;
- avoid `float`/`double` unless FPU support is specified;
- avoid exceptions and RTTI unless explicitly permitted;
- avoid STL containers that allocate dynamically;
- use fixed-size buffers, static allocation, ring buffers, or RTOS memory pools;
- use RTOS task/mutex/timer APIs only when the blueprint names the RTOS or declares the API.

If the RTOS is not specified and an RTOS-specific API is required, trigger Step 3.3.

## GUI

C/C++ GUI anchors require `TARGET_UI_FRAMEWORK`.

Common explicit values:

- `Win32`
- `Qt`
- `GTK`
- `Cocoa`
- `custom:<name>`

Do not choose a GUI toolkit silently.

## Database

C/C++ database code requires `DATABASE_PROVIDER` and usually `DATABASE_DRIVER` or `USES`.

Examples when explicitly declared:

- SQLite: `sqlite3`
- PostgreSQL: `libpq`
- MySQL/MariaDB: `libmysqlclient` or `mariadb-connector-c`
- ODBC: platform ODBC library

Do not add ORM or wrapper libraries unless declared.

## Platform Notes

- Windows API code must not be generated for Linux/macOS.
- POSIX APIs must not be generated for Windows without compatibility layers.
- Use `std::filesystem` only for C++17+ desktop/server targets; report fallback otherwise.
