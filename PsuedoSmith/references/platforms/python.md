# Platform Reference: Python

## Default Version

Default `TARGET_LANGUAGE_VERSION`: Python 3.11 unless the blueprint specifies otherwise.
If compatibility with older enterprise runtimes is implied, use Python 3.8-compatible syntax
and report the fallback.

## Imports

Order imports:

1. standard library;
2. third-party dependencies declared in `USES` or header fields;
3. local modules.

## Data Types

- `STRING` -> `str`
- `INT` -> `int`
- `FLOAT`/`DOUBLE` -> `float`
- `DECIMAL` -> `decimal.Decimal`
- `BOOLEAN` -> `bool`
- `LIST` -> `list`
- `COLLECTION` -> `set`
- `DICTIONARY` -> `dict`
- `DATETIME` -> `datetime.datetime`
- `CURRENCY` -> `decimal.Decimal`; never `float`/`double` for monetary values.

Use type hints unless the blueprint or style guide says not to.

## Database

Standard-library database support exists for SQLite via `sqlite3`.

For PostgreSQL, MySQL/MariaDB, SQL Server, Oracle, or async database access, require a
`DATABASE_DRIVER` or declared `USES` dependency. If absent, trigger Step 3.3.

Typical driver names, when explicitly declared:

- SQLite: `sqlite3`
- PostgreSQL: `psycopg`, `psycopg2`, `asyncpg`
- MySQL/MariaDB: `mysql-connector-python`, `PyMySQL`, `aiomysql`
- SQL Server: `pyodbc`
- Oracle: `oracledb`

## Async and Concurrency

- `ASYNC`/`AWAIT` -> `async def` and `await`.
- Threads -> `threading.Thread`.
- Locks -> `threading.Lock` or `asyncio.Lock` depending on context.
- Do not introduce an async DB/network dependency unless declared.

## GUI

Python has no single standard GUI for all targets. GUI anchors require `TARGET_UI_FRAMEWORK`
or an explicit framework in `USES`.

Common explicit frameworks:

- `Tkinter` (standard library, desktop only);
- `PyQt`/`PySide`/`Qt`;
- `Kivy`;
- `Textual`;
- `custom:<name>`.

Do not choose one silently.

## CPython vs MicroPython / embedded_rtos

For `TARGET_PLATFORM = embedded_rtos` or a blueprint indicating MicroPython:

| CPython feature | MicroPython substitute / rule |
|---|---|
| `decimal.Decimal` | fixed-point integer arithmetic |
| `datetime` | `utime` |
| `threading` | cooperative scheduling or limited `uasyncio` |
| `os.path` | `uos` |
| `re` | limited `ure` or explicit loops |
| `json` | `ujson` |
| arbitrary file system | board/filesystem-specific APIs |

Avoid heap-heavy structures unless explicitly requested.

## Version Fallbacks

- Python `match` requires 3.10+. Use `if/elif/else` for older targets.
- `zoneinfo` requires 3.9+. Do not add `pytz` unless declared.
- Avoid Python 2 generation; if requested, report unsupported/end-of-life.
