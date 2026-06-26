# Header Schema

Every UAB blueprint should begin with a header enclosed in `/* ... */` or a similar block.
The header is the control plane for translation.

## Extensible Header Values
The listed values are recognized defaults, not an exhaustive list. Unknown, unlisted, or custom values are allowed when the translator can reasonably generate code for the requested target using the blueprint and available context. If the value materially affects implementation and the translator lacks enough platform-specific information to proceed safely, trigger the material ambiguity checkpoint.

## Required Fields

| Field | Required | Values |
|---|---:|---|
| `TARGET_LANGUAGE` | Yes | `Python`, `C`, `C++`, `C#`, `Java`, `Kotlin`, `Swift`, `JavaScript`, `TypeScript`, `VBA`, `PHP`, `Rust`,  ... |
| `TARGET_PLATFORM` | Yes | `win32`, `win64`, `linux_x86`, `linux_arm`, `mac_x64`, `mac_arm`, `android_arm64`, `ios_arm64`, `embedded_rtos`, `wasm`, `custom:<name>`, ... |

Do not use `JavaScript/TS`. `JavaScript` and `TypeScript` are separate targets.

## Optional Core Fields

| Field | Values | Default / Rule |
|---|---|---|
| `TARGET_LANGUAGE_VERSION` | version specifier, range, `latest`, `current` | Use platform reference default if omitted. |
| `MEMORY_MODEL` | `garbage_collected`, `manual_ownership`, `automatic_ref_counting` | Infer from language if omitted. |
| `CONCURRENCY_MODEL` | `async_await`, `pthreads`, `std_thread`, `coroutines`, `single_threaded`, `rtos_tasks` | Infer from explicit concurrency anchors or platform default. |
| `STYLE_GUIDE` | relative path or named style guide | Optional. Overrides inline style defaults when provided. |
| `NAMING_CONVENTIONS` | `PascalCase`, `camelCase`, `snake_case`, `kebab-case`, `language_default` | Use idiomatic target-language naming if omitted. |
| `INDENTATION` | `spaces:4`, `spaces:2`, `tabs` | Default `spaces:4` unless target ecosystem strongly implies otherwise. |
| `DELIVERY_MODE` | `archive`, `inline`, `both` | Default `archive` |

## Delivery Clarification Rule

If `DELIVERY_MODE` is omitted, treat it as `archive`.

- `archive` — all generated source delivered as a single downloadable `.zip`, byte-exact,
  mirroring the blueprint file layout (DEFAULT).
- `inline` — source rendered in the response. Transport-lossy; may strip operators or inject
  invisible characters (zero-width / BOM). Use only by explicit choice.
- `both` — inline rendering AND a downloadable archive.

The header value **pre-selects the default at the delivery checkpoint; it does not remove the
checkpoint.** The translator states the resolved mode and lets the engineer confirm or
override, so the delivery choice is never taken from the engineer.

Regardless of the declared value, delivery is **upgraded to at least `both`** when the output
is multi-file or requires a specific on-disk build structure (e.g. an Avalonia/.NET project
with `*.csproj`, an entry point, and modules). Pure `inline` is never valid for a
build-structured project; record the upgrade reason in the Implementation Report.

An unrecognized value is a material header error: ask the engineer to correct it at Step 3.3.
Do not silently coerce.

## Archive Integrity

When delivering an archive, the translator must:

- Mirror the blueprint file layout with correct relative paths.
- Write source as UTF-8 **without BOM**, using `\n` line endings unless the target platform
  requires otherwise.
- Preserve byte-exact contents (no reflow or prettify during packaging).
- Name the archive after the module/project (e.g. `CCValidator.zip`).
- Include `IMPLEMENTATION_REPORT.md` at the archive root when report export was chosen.

## No-Filesystem Fallback

If the runtime cannot emit a downloadable artifact, the translator must (1) state that
`archive`/`both` is unavailable, (2) fall back to `inline`, and (3) emit an integrity
advisory instructing the engineer to retype event/string literals by hand and to scan for
invisible characters, e.g.:

## Optional Database Fields

| Field | Values | Rule |
|---|---|---|
| `DATABASE_PROVIDER` | `sqlite`, `postgresql`, `mysql`, `mariadb`, `sqlserver`, `oracle`, `odbc`, `custom:<name>` | Optional globally, conditionally required when database generation or persistence is requested. |
| `DATABASE_ACCESS` | `sync`, `async` | Optional. Infer from explicit `ASYNC`, `AWAIT`, or `CONCURRENCY_MODEL` if obvious; otherwise use the simplest idiomatic synchronous access. |
| `DATABASE_DRIVER` | driver/library/module name | Optional. Counts as a declared dependency. Required at Step 3.3 if no standard driver exists and no `USES` driver is declared. |

### Database Clarification Rule

If the blueprint contains `DATABASE.*` anchors or natural-language database/persistence
requirements, `DATABASE_PROVIDER` is conditionally required. File-based persistence should use file anchors or platform file APIs and does not require DATABASE_PROVIDER.

If missing, pause at Step 3.3 and ask which provider to target. Do not generate database-
specific SQL, connection code, migrations, or provider-specific types until the provider is
known.

If a non-standard driver is needed and neither `DATABASE_DRIVER` nor `USES` declares one,
pause at Step 3.3 before adding the dependency.

## Optional UI Field

| Field | Values | Rule |
|---|---|---|
| `TARGET_UI_FRAMEWORK` | `none`, `WinForms`, `WPF`, `MAUI`, `Avalonia`, `SwiftUI`, `UIKit`, `AppKit`, `Swing`, `JavaFX`, `AndroidCompose`, `React`, `HTML`, `UNO`, `Qt`, `GTK`, `egui`, `iced`, `slint`, `custom:<name>` | Optional, default `none`. Conditionally required when GUI anchors appear. |

If GUI anchors appear and `TARGET_UI_FRAMEWORK` is missing or `none`, pause at Step 3.3.
Do not choose a framework silently.

## Optional Feature Flags

Optional features are opt-in. Do not perform them unless the parent flag is `TRUE`.

| Field | Values | Default | Reference |
|---|---|---:|---|
| `GENERATE_UNIT_TESTS` | `TRUE`, `FALSE` | `FALSE` | `references/optional-features/tests.md` |
| `TEST_FRAMEWORK` | framework name | auto-detect only if tests enabled | `references/optional-features/tests.md` |
| `TEST_COVERAGE` | `NONE`, `CRITICAL`, `ALL` | `CRITICAL` | `references/optional-features/tests.md` |
| `TEST_OUTPUT_DIR` | relative path | language standard only if tests enabled | `references/optional-features/tests.md` |
| `GENERATE_DOCSTRINGS` | `TRUE`, `FALSE` | `FALSE` | `references/optional-features/docstrings.md` |
| `DOCSTRING_STYLE` | style name | auto-detect only if docstrings enabled | `references/optional-features/docstrings.md` |
| `DOCSTRING_COVERAGE` | `NONE`, `PUBLIC_ONLY`, `ALL` | `PUBLIC_ONLY` | `references/optional-features/docstrings.md` |
| `GENERATE_DEPENDENCY_GRAPH` | `TRUE`, `FALSE` | `FALSE` | `references/optional-features/dependency-graph.md` |
| `DEPENDENCY_GRAPH_FORMAT` | `mermaid`, `dot`, `plantuml`, `text` | `mermaid` | `references/optional-features/dependency-graph.md` |
| `FAIL_ON_CIRCULAR_DEPENDENCY` | `TRUE`, `FALSE` | `TRUE` | `references/optional-features/dependency-graph.md` |
| `DETECT_DEAD_CODE` | `TRUE`, `FALSE` | `FALSE` | `references/optional-features/dead-code.md` |

Do not ask about sub-flags unless the engineer enables or asks about the parent feature.

## Header Example

```text
/*
 ====
 TARGET_LANGUAGE         : Python
 TARGET_PLATFORM         : linux_x86
 TARGET_LANGUAGE_VERSION : >=3.11
 MEMORY_MODEL            : garbage_collected
 CONCURRENCY_MODEL       : async_await

 DATABASE_PROVIDER       : postgresql
 DATABASE_ACCESS         : async
 DATABASE_DRIVER         : asyncpg

 TARGET_UI_FRAMEWORK     : none

 STYLE_GUIDE             : ./styleguide.md
 NAMING_CONVENTIONS      : snake_case
 INDENTATION             : spaces:4

 GENERATE_UNIT_TESTS       : FALSE
 GENERATE_DOCSTRINGS       : FALSE
 GENERATE_DEPENDENCY_GRAPH : FALSE
 DETECT_DEAD_CODE          : FALSE
 
 DELIVERY_MODE           : archive
 ====
*/
```

## Header Validation Rules

1. Missing required fields must be requested before translation.
2. Optional fields should not be requested unless they are material to the blueprint.
3. Conditional requirements are handled at Step 3.3.
4. Header fields override inferred defaults.
5. Blueprint body may refine but not contradict the header. If contradiction is material,
   trigger Step 3.3.
