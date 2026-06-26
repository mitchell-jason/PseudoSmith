# Platform Reference: JavaScript and TypeScript

## Language Split

`JavaScript` and `TypeScript` are separate `TARGET_LANGUAGE` values.

- JavaScript output should be `.js` unless the blueprint says otherwise.
- TypeScript output should be `.ts` and include type annotations.
- Do not emit TypeScript syntax for JavaScript targets.

## Default Version

Default JavaScript/TypeScript target: ES2020-compatible syntax unless
`TARGET_LANGUAGE_VERSION` specifies otherwise.

## Monetary Values

- `CURRENCY` -> JavaScript/TypeScript have no native decimal type. Use integer minor units
  (e.g. cents) or a declared decimal library (e.g. `decimal.js`, `dinero.js`) via `USES`.
  Never use `number`/floating point for monetary math.

## Module System

If the blueprint specifies a module system, follow it.

If unspecified:

- browser/wasm targets: default to ES modules;
- Node.js targets: prefer ES modules for new code, but do not create `package.json` unless
  requested or required by downloadable project output.

Do not use `require()` for browser targets.

## Browser/WASM vs Node.js

| Node.js-only feature | Browser/WASM substitute / rule |
|---|---|
| `fs` | no arbitrary local file access; use `fetch`, File API, IndexedDB, or TODO depending on blueprint |
| `process.env` | build-time injection only; do not emit directly |
| `Buffer` | `Uint8Array`, `ArrayBuffer`, `TextEncoder`, `TextDecoder` |
| `child_process` | unavailable |
| `net` / `dgram` | `WebSocket` only when semantically acceptable |
| CommonJS `require` | ES modules |
| `__dirname` / `__filename` | `import.meta.url` or omit |
| Node `crypto` | Web Crypto API (`crypto.subtle`) in browser |

Browser-only APIs such as `document`, `window`, `HTMLElement`, `localStorage`, and
`IndexedDB` must not be generated for Node.js targets.

## Database

JavaScript/TypeScript have no universal standard database driver.

If `DATABASE_PROVIDER` is set, require `DATABASE_DRIVER` or `USES` for the selected runtime.
Examples when explicitly declared:

- SQLite: `better-sqlite3`, `sqlite3`, `sql.js`
- PostgreSQL: `pg`, `postgres`, `kysely` with driver
- MySQL/MariaDB: `mysql2`
- SQL Server: `mssql`
- Browser storage: `IndexedDB` only when provider/intent indicates browser persistence

If missing, trigger Step 3.3.

## GUI

GUI anchors require `TARGET_UI_FRAMEWORK`.

Common explicit values:

- `HTML`
- `React`
- `Vue`
- `Svelte`
- `Angular`
- `custom:<name>`

If `TARGET_UI_FRAMEWORK = HTML`, generate plain HTML/CSS/JS or TS-compatible DOM code.
If `TARGET_UI_FRAMEWORK = React`, generate JSX/TSX only when appropriate to the target.

## TypeScript Rules

- Prefer explicit interfaces/types for `STRUCT`, `INTERFACE`, and DTO-like shapes.
- Use `unknown` rather than `any` for untrusted values unless the blueprint permits loose typing.
- Do not create a build configuration unless requested or necessary for downloadable project output.
- `DELEGATE` -> a function type alias (e.g. `type Handler = (x: T) => void`). In plain JavaScript, document the expected callable signature in JSDoc.

## Security and Crypto

Node and browser crypto APIs are materially different. If `HASH`, `ENCRYPT`, `SIGN`, or
similar anchors appear without a specified algorithm/policy, trigger Step 3.3.
