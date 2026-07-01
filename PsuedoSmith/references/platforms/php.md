# Platform Reference: PHP

## Default Version

Default: PHP 8.1+ unless the blueprint specifies PHP 7.x or another target.

If PHP 7 compatibility is specified or implied, avoid PHP 8-only features and report
fallbacks.

## Types and Syntax

- Use scalar type hints and return types when target version permits.
- Use nullable types (`?Type`) where appropriate.
- Use union types only for PHP 8.0+.
- Use `match` only for PHP 8.0+; otherwise use `switch`.
- Use enums only for PHP 8.1+; otherwise use class constants.
- `DELEGATE` -> a `callable` type hint, or `Closure` when a first-class closure is intended.

## Runtime Context

PHP can run as CLI or web SAPI. `TARGET_PLATFORM` alone may not fully specify this.

If the blueprint relies on HTTP request/response globals, sessions, cookies, or CLI args and
the runtime context is unclear, trigger Step 3.3.

## GUI / Web Output

PHP has no native GUI; When GUI anchors appear and no
`TARGET_UI_FRAMEWORK` is declared, default to HTML/CSS and report the assumption (a PHP-specific
default; the general anchor rule still requires an explicit framework).

Map controls to HTML/CSS:

- `WINDOW`, `DIALOG`, `PANEL` -> `<div>` containers;
- `BUTTON` -> `<button>`;
- `TEXTBOX`/`INPUT` -> `<input>`;
- `TEXTAREA` -> `<textarea>`;
- `DROPDOWN`/`COMBOBOX` -> `<select>`;
- `TABLE`/`DATAGRID` -> `<table>`.

If the blueprint implies a client-side SPA or JSON API rather than server-rendered markup, that is
material — surface it at Step 3.3 instead of assuming server HTML.

## Database

PHP idiom is PDO with prepared statements. Drivers are provider-specific and environment-dependent.

When `DATABASE_PROVIDER` is not declared, default to MySQL (`pdo_mysql`) and confirm the choice at
Step 3.3 (default + confirm, not a hard block). PDO is the access layer regardless of provider; the
provider selects the DSN and SQL dialect.
Common provider mappings:

- SQLite: `pdo_sqlite`;
- PostgreSQL: `pdo_pgsql`;
- MySQL/MariaDB: `pdo_mysql`;
- SQL Server: `pdo_sqlsrv`;
- Oracle: `oci8` or PDO driver if specified.

If the driver is unclear or not declared, trigger Step 3.3.

## Security Anchors

- For `SANITIZE ... FOR HTML`, use `htmlspecialchars` with appropriate flags.
- For SQL, use prepared statements.
- Do not invent auth/session/password policy. Follow `references/security.md`.

## Types and Syntax

- `CURRENCY` -> integer minor units (e.g. cents) or the `bcmath`/`decimal` extension when declared; never `float` for monetary values.
