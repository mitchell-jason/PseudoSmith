# Platform Reference: Google Apps Script

## Scope

This file covers Google Apps Script, abbreviated as GAS.

Canonical target language value:

- `GoogleAppsScript`

Accepted aliases:

- `GAS`
- `Google Apps Script`

Google Apps Script is a JavaScript-based runtime hosted by Google Workspace. It is not Node.js and it is not browser JavaScript, although HTML Service client files run in the browser.

Load this reference when the blueprint targets Google Apps Script, GAS, Apps Script, or Google Workspace automation.

## Defaults

Default runtime: Apps Script V8 runtime.

Default source extension:

- server-side Apps Script files: `.gs`
- HTML Service files: `.html`
- manifest file, when generated: `appsscript.json`

Do not generate Node.js, npm, package.json, bundler, CommonJS, or filesystem-based project artefacts unless the blueprint explicitly requests an external build/transpile workflow.

## Runtime Model

Google Apps Script code runs in Google's hosted Apps Script environment.

Important constraints:

- no Node.js standard library;
- no `fs`;
- no `path`;
- no `process`;
- no `Buffer` unless explicitly polyfilled in scope;
- no npm packages unless an external clasp/build/transpile workflow is explicitly requested;
- no raw sockets;
- no arbitrary local filesystem access;
- services are generally synchronous;
- execution time and quota limits apply.

Use Apps Script services instead of Node/browser APIs where appropriate.

Common built-in services include:

- `SpreadsheetApp`
- `DocumentApp`
- `DriveApp`
- `GmailApp`
- `CalendarApp`
- `SlidesApp`
- `FormsApp`
- `UrlFetchApp`
- `PropertiesService`
- `CacheService`
- `LockService`
- `ScriptApp`
- `HtmlService`
- `Session`
- `Utilities`
- `Logger`

Use only services required by the blueprint. Do not add unrelated Google Workspace service usage.

## Files and Project Layout

File layout is blueprint-owned.

If no file layout is specified, generate the minimal `.gs` file set required.

Do not generate `appsscript.json` unless one of the following is true:

- the blueprint requests a manifest;
- explicit OAuth scopes are required and requested;
- advanced Google services are requested;
- web app deployment settings are requested;
- add-ons, triggers, or special runtime settings are requested;
- project/package output is requested.

If a manifest is not generated but scopes or deployment notes are relevant, include them in the Implementation Report.

## Language Rules

Apps Script uses JavaScript syntax on the V8 runtime.

Mapping rules:

- `MODULE` -> logical `.gs` file or grouped functions/classes.
- `CLASS` -> JavaScript `class`.
- `STRUCT` -> plain object shape or class depending on blueprint intent.
- `INTERFACE` -> not natively supported in JavaScript; generate JSDoc typedef/contract comments when needed.
- `FUNCTION` / `PROCEDURE` -> function declarations.
- `PROPERTY` -> object property or class getter/setter.
- `LIST` / `ARRAY` -> `Array`.
- `DICTIONARY` -> plain object or `Map`, depending on key requirements.
- `DATETIME` -> JavaScript `Date`.
- `CURRENCY` -> no native decimal type; use integer minor units (e.g. cents) or a declared decimal library. Never use floating point for monetary math.
- `DELEGATE` -> a function value (JavaScript functions are first-class); document the expected signature in JSDoc.
- `NULL` / `NIL` -> `null`.

Do not emit TypeScript syntax for `GoogleAppsScript` unless the blueprint explicitly requests TypeScript-to-Apps-Script output.

## Global Function Requirements

Many Apps Script entry points must be globally visible functions, including:

- simple triggers such as `onOpen`, `onEdit`, `doGet`, `doPost`;
- installable trigger handlers;
- menu callbacks;
- functions called from `google.script.run`;
- custom spreadsheet functions.

Do not rename these functions in a way that breaks Apps Script discovery.

Dead-code detection must not flag trigger handlers, menu callbacks, custom functions, web-app handlers, or `google.script.run` targets solely because they have no local call site.

## Triggers and Scheduling

Apps Script supports simple triggers and installable triggers.

Do not create triggers unless the blueprint explicitly requests them.

Relevant anchors:

- `WHENLOADED` or spreadsheet open behavior may map to `onOpen(e)` when the blueprint targets Spreadsheet-bound scripts.
- `WHENCHANGED` or edit behavior may map to `onEdit(e)` only when spreadsheet edit semantics are intended.
- `CRON`, `TIMER`, or scheduled behavior may map to installable time-driven triggers only when requested.

If scheduling is requested but trigger type, cadence, or authorization requirements are unclear, trigger the material ambiguity checkpoint.

## Google Workspace UI

GUI anchors require `TARGET_UI_FRAMEWORK`.

Common explicit values:

- `HtmlService`
- `SpreadsheetUI`
- `DocumentUI`
- `SlidesUI`
- `FormsUI`
- `CardService`
- `custom:<name>`

Rules:

- For Spreadsheet/Docs/Slides menus, use host UI APIs such as `SpreadsheetApp.getUi()`, `DocumentApp.getUi()`, or `SlidesApp.getUi()`.
- For dialogs and sidebars, use `HtmlService` when selected.
- For Google Workspace Add-ons, use `CardService` when selected.
- Do not choose a UI framework silently.

If host application is unclear and GUI behavior depends on it, trigger the material ambiguity checkpoint.

## HTML Service

When `TARGET_UI_FRAMEWORK = HtmlService`, generate:

- `.gs` server-side functions;
- `.html` files only when UI markup is in scope;
- `google.script.run` calls for client-to-server communication.

Client-side JavaScript inside `.html` runs in the browser sandbox. Server-side `.gs` code runs in Apps Script.

Do not use DOM APIs in `.gs` files. Do not use Apps Script services directly inside client browser JavaScript.

## Spreadsheet Behavior

When targeting Google Sheets:

- use `SpreadsheetApp`;
- use `getRange`, `getValues`, and `setValues` for batch operations;
- avoid row-by-row service calls when batch operations are practical;
- check for missing sheets, empty ranges, and malformed data;
- preserve formulas unless the blueprint says to overwrite them.

If the spreadsheet structure, sheet names, header rows, or range layout are material and unspecified, trigger the material ambiguity checkpoint.

## Drive and File Storage

Apps Script cannot access arbitrary local files.

For Google Drive files, use `DriveApp` only when Drive access is requested.

For script-local configuration or small persisted values, use:

- `PropertiesService.getScriptProperties()`;
- `PropertiesService.getUserProperties()`;
- `PropertiesService.getDocumentProperties()` when document-bound behavior is intended.

For temporary caching, use `CacheService`.

Do not silently choose Drive, PropertiesService, CacheService, or spreadsheet-backed storage as a database substitute unless the blueprint intent clearly supports it.

## Network

Use `UrlFetchApp` for HTTP(S) requests.

Do not generate raw socket code.

If external API behavior is requested, require enough information for:

- URL or endpoint;
- HTTP method;
- headers/authentication requirements;
- request body format;
- response handling.

If authentication or token handling is unspecified and material, trigger the material ambiguity checkpoint.

## Database and Persistence

Apps Script has no general local database engine.

Database behavior requires `DATABASE_PROVIDER`.

Possible persistence mechanisms, only when explicitly requested or clearly intended:

- Spreadsheet-backed storage;
- PropertiesService;
- CacheService;
- Drive files;
- JDBC service for supported external databases;
- external API-backed persistence.

JDBC/database use requires provider, connection details, and authentication mechanism. If these are missing, trigger the material ambiguity checkpoint.

Do not invent a database provider or silently convert database requirements into spreadsheet storage unless the blueprint says to do so.

## Advanced Google Services

Advanced Google services must be explicitly declared.

Examples:

- Google Sheets API advanced service;
- Google Drive API advanced service;
- Admin SDK;
- People API;
- Gmail API advanced service.

Do not replace built-in services with advanced services unless requested or necessary for the specified behavior.

If an advanced service is required, report the requirement and generate manifest/service configuration only when project/manifest output is in scope.

## Security and Authorization

Apps Script uses Google authorization scopes.

Do not invent:

- OAuth consent configuration;
- external OAuth flows;
- service account behavior;
- domain-wide delegation;
- secret storage policy;
- token refresh policy;
- add-on publication settings.

Use `PropertiesService` for simple script/user properties only when the blueprint requests or clearly implies persisted configuration. Do not store sensitive secrets there unless the blueprint explicitly accepts that policy; record a risk warning when relevant.

If a service requires authorization scopes and manifest generation is not in scope, list the likely required scopes in the Implementation Report instead of generating unrelated project files.

## Concurrency and Locks

Apps Script executions may overlap.

Use `LockService` only when the blueprint requests shared mutable state, concurrent trigger protection, or a race condition is clearly present.

Do not add locking everywhere by default.

If consistency requirements are unclear and materially affect implementation, trigger the material ambiguity checkpoint.

## Quotas and Long-Running Work

Apps Script has execution time, service quota, and rate limits.

For large spreadsheet, Drive, Gmail, Calendar, or network operations:

- prefer batch APIs where available;
- avoid repeated service calls inside tight loops;
- checkpoint or emit TODOs when required volume exceeds safe assumptions.

Do not create continuation triggers, queues, batching frameworks, or retry systems unless the blueprint requests them.

## Logging and Errors

Use:

- `console.log` for modern Apps Script logging;
- `Logger.log` when simpler legacy logging is appropriate.

For production paths, prefer throwing errors or returning structured results according to blueprint intent.

Wrap external service calls, network calls, and data mutations in error handling.

## Testing

Apps Script has no built-in standard unit test runner.

If `GENERATE_UNIT_TESTS = TRUE`, generate one of:

- simple assertion functions in `.gs`;
- a declared test framework only if specified;
- testable pure helper functions separated from Apps Script service calls.

Do not add third-party Apps Script test frameworks unless declared.

## Platform Compatibility Audit

Before presenting output, audit generated code for:

- Node.js APIs;
- browser APIs in `.gs`;
- Apps Script services inside client `.html` JavaScript;
- undeclared advanced services;
- undeclared manifest/scopes;
- trigger handlers incorrectly scoped or renamed;
- per-cell/per-row service calls that should be batched;
- missing null checks for sheets, ranges, files, labels, events, and properties;
- unauthorized service usage not mentioned in the report.

Replace violations with Apps Script-native equivalents when unambiguous. Otherwise emit TODO comments and report partial implementation.

## TODO Comment Syntax

Use JavaScript comments:

```text
// TODO: description
```
