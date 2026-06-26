# Platform Reference: Swift

## Scope

This file covers Swift source generation for Apple platforms and Swift-supported non-Apple targets.

Common target platforms include:

- `ios_arm64`
- `mac_x64`
- `mac_arm`
- `linux_x86`
- `linux_arm`
- `wasm`
- `custom:<name>`

Swift platform behavior varies significantly between iOS, macOS, Linux, server-side Swift, embedded/custom targets, and WebAssembly. If platform-specific behavior is required and the blueprint does not provide enough context, trigger the material ambiguity checkpoint.

## Defaults

Default language target: Swift 5.9-compatible syntax unless `TARGET_LANGUAGE_VERSION` specifies otherwise.

Use Swift 6 strict-concurrency features only when the blueprint or target version explicitly requests Swift 6 behavior.

Default memory model: `automatic_ref_counting`.

Default naming conventions:

- types: `PascalCase`
- functions, methods, properties, and variables: `camelCase`
- enum cases: `camelCase`

## Files and Modules

Swift file layout is blueprint-owned.

If no file layout is specified, generate the minimal idiomatic file set required.

Do not create an Xcode project, Swift Package Manager manifest, package layout, entitlements, asset catalog, Info.plist, deployment configuration, or build settings unless the blueprint requests them.

If the blueprint requests a Swift package or executable package, generating `Package.swift` is in scope.

## Imports and Dependencies

The Swift standard library is allowed.

Use `Foundation` for common platform functionality such as:

- `Date`
- `UUID`
- `URL`
- `URLSession`
- `FileManager`
- `Data`
- `Decimal`
- JSON encoding and decoding

Do not add third-party packages unless declared with `USES`, a header field, or explicit blueprint policy.

Do not add frameworks such as SwiftUI, UIKit, AppKit, Combine, CryptoKit, Network, CoreData, or SwiftData unless they are selected by the blueprint, required by the selected platform/UI framework, or explicitly declared.

Platform frameworks must be audited against `TARGET_PLATFORM`.

## Data Types

Common mappings:

| UAB | Swift |
|---|---|
| `STRING` | `String` |
| `INT` | `Int` |
| `SHORT` | `Int16` when size matters |
| `LONG` | `Int64` when size matters |
| `UNSIGNED` | `UInt` or fixed-width `UInt*` when size matters |
| `BYTE` | `UInt8` |
| `FLOAT` | `Float` |
| `DOUBLE` | `Double` |
| `DECIMAL` | `Decimal` via Foundation |
| `BOOLEAN` | `Bool` |
| `CHAR` | `Character` |
| `DATE` / `DATETIME` | `Date` via Foundation |
| `GUID` / `UUID` | `UUID` via Foundation |
| `ARRAY` / `LIST` | `[T]` |
| `COLLECTION` | `Set<T>` when `T: Hashable` |
| `DICTIONARY` | `[Key: Value]` |
| `NULL` / `NIL` | `nil` |
| `CURRENCY` | `Decimal` via Foundation; never `Float`/`Double` for monetary values |

Use optionals for values that may be absent.

Do not force unwrap with `!` unless the blueprint explicitly guarantees non-null behavior and panic/crash behavior is acceptable. Prefer `guard let`, `if let`, optional chaining, or throwing errors.

## Classes, Structs, Protocols, and Enums

Mapping rules:

- `CLASS` -> `class`
- `STRUCT` -> `struct`
- `INTERFACE` -> `protocol`
- `DELEGATE` -> a function type / closure (e.g. `typealias Handler = (T) -> Void`). Note: this is the UAB callable-type anchor, not the Cocoa delegate pattern; use a `protocol` only if the blueprint describes the delegation pattern explicitly.
- `ENUM` -> `enum`
- `PROPERTY` -> stored or computed property depending on blueprint intent
- `CONSTRUCTOR` -> `init`
- `DESTRUCTOR` -> `deinit` for classes only

Prefer `struct` for value-oriented DTOs only when the blueprint uses `STRUCT` or value semantics are clearly intended. Do not silently convert `CLASS` to `struct`.

Use access modifiers according to blueprint visibility:

- `PUBLIC` -> `public` when cross-module API is intended; otherwise default internal may be more idiomatic for single-module code
- `PRIVATE` -> `private`
- `PROTECTED` -> no direct Swift equivalent; use `internal`/`fileprivate` and report fallback
- `INTERNAL` -> `internal`

Swift has no direct `protected` access modifier. Any mapping from `PROTECTED` must be recorded in the Implementation Report.

## Error Handling

Use Swift error handling for fallible operations:

- define `Error` enums when useful and in scope;
- use `throws`;
- use `try`, `try?`, or `do/catch` according to blueprint intent.

Do not use `fatalError`, forced unwraps, or unchecked assumptions in production paths unless the blueprint explicitly requests fail-fast behavior.

## Async and Concurrency

`ASYNC` and `AWAIT` map to Swift `async` functions and `await` when the target version supports Swift concurrency.

Use:

- `async throws` for asynchronous fallible operations;
- `Task` only when task creation is explicitly needed;
- `actor` only when actor isolation is requested or clearly required by the concurrency model.

Do not introduce Combine, RxSwift, Promise libraries, or third-party concurrency frameworks unless declared.

For Swift 6 or strict concurrency contexts, respect `Sendable`, actor isolation, and main actor requirements. If the target concurrency mode is unclear and materially affects public interfaces, trigger the material ambiguity checkpoint.

## GUI

GUI anchors require `TARGET_UI_FRAMEWORK`.

Common explicit values:

- `SwiftUI`
- `UIKit`
- `AppKit`
- `WatchKit`
- `TVUIKit`
- `custom:<name>`

Rules:

- `UIKit` is for iOS/tvOS-style targets, not macOS-only targets unless Catalyst is specified.
- `AppKit` is for macOS.
- `SwiftUI` can target multiple Apple platforms but still requires platform/version awareness.
- Do not choose a UI framework silently.

If GUI behavior requires lifecycle files, app entry points, scenes, storyboards, nibs, asset catalogs, entitlements, or project settings, generate them only when the blueprint requests a full app/project output or explicitly includes those artefacts.

## File System

Use `FileManager`, `URL`, and `Data` for file I/O on Foundation-capable platforms.

Use app-container-safe paths on iOS and sandboxed macOS when relevant.

Do not assume arbitrary filesystem access on iOS, sandboxed macOS, or wasm targets.

If file access location, sandbox behavior, security-scoped resource handling, or entitlements are material and unspecified, trigger the material ambiguity checkpoint.

## Network

Use `URLSession` for HTTP(S) requests when Foundation networking is available and the blueprint requests HTTP behavior.

Do not use raw sockets unless the blueprint explicitly requests socket-level networking and the required framework/API is available for the target platform.

For Apple Network framework usage, require explicit declaration or platform context.

## Database and Persistence

Swift has no universal standard SQL database layer.

Database behavior requires `DATABASE_PROVIDER`.

Provider-specific drivers, wrappers, ORMs, or persistence frameworks must be declared.

Examples that require explicit declaration or clear blueprint context:

- SQLite via `SQLite3` C API;
- GRDB;
- SQLite.swift;
- CoreData;
- SwiftData;
- PostgreSQL client libraries;
- MySQL client libraries;
- Realm.

Do not add CoreData, SwiftData, SQLite wrappers, ORMs, or server-side database clients unless requested or declared.

For simple local persistence, if explicitly requested and appropriate, use standard platform storage such as:

- `UserDefaults` for small preference-like values;
- file-based JSON/plist storage via Foundation;
- Keychain only when explicitly requested for secrets.

Do not silently choose Keychain, CoreData, SwiftData, or SQLite.

## Security and Crypto

Security-sensitive behavior must follow `references/security.md`.

Do not invent encryption, signing, token, password, Keychain, Secure Enclave, biometric, certificate-pinning, or key-management policy.

`CryptoKit` is Apple-platform-oriented and availability varies. Use it only when declared, when the platform supports it, and when the algorithm/policy is specified.

If a requested security operation lacks an algorithm, key-management policy, or storage policy and the choice materially changes implementation, trigger the material ambiguity checkpoint.

## Platform Compatibility Audit

Before presenting output, audit generated code for:

- using UIKit on non-iOS targets;
- using AppKit on non-macOS targets;
- using SwiftUI without selected UI framework;
- using Foundation APIs unavailable on the selected target;
- using Swift concurrency features below Swift 5.5;
- using Swift 6-only behavior when not requested;
- using Apple-only frameworks on Linux/server targets;
- assuming arbitrary filesystem access on iOS, sandboxed macOS, or wasm targets;
- adding package dependencies without declaration.

Replace with native equivalents when unambiguous. Otherwise emit TODO comments and report partial implementation.

## TODO Comment Syntax

Use Swift comments:

```text
// TODO: description
```
