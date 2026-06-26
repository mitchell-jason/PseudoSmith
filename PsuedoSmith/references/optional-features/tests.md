# Optional Feature: Unit Tests

Run only when `GENERATE_UNIT_TESTS = TRUE`.

Do not mention, generate, or scaffold tests for simple translations unless the flag is enabled
or the user explicitly asks.

## Header Fields

| Field | Values | Default |
|---|---|---|
| `GENERATE_UNIT_TESTS` | `TRUE`, `FALSE` | `FALSE` |
| `TEST_FRAMEWORK` | framework name | auto-detect only when tests enabled |
| `TEST_COVERAGE` | `NONE`, `CRITICAL`, `ALL` | `CRITICAL` |
| `TEST_OUTPUT_DIR` | relative path | language standard |

`TEST_FRAMEWORK` counts as a declared dependency only when the tests feature is enabled.

## Framework Defaults

Use these defaults only when tests are enabled and no framework is specified:

| Language | Default | Fallback |
|---|---|---|
| Python | `unittest` | `pytest` only if declared or requested |
| C# | xUnit if declared/project context supports it; otherwise MSTest-style scaffold | manual test harness |
| Java | JUnit 5 if declared/project context supports it | JUnit 4 or manual scaffold |
| Kotlin | kotlin.test or JUnit if declared | manual scaffold |
| C/C++ | manual test harness unless framework declared | GoogleTest/Catch2 only if declared |
| JavaScript | Jest/Vitest only if declared | simple assertion harness |
| TypeScript | Jest/Vitest only if declared | simple assertion harness |
| Swift | XCTest | manual scaffold if unavailable |
| PHP | PHPUnit if declared | manual scaffold |
| Rust | built-in `#[test]` | none |
| VBA | manual debug stubs | none |

Do not add a third-party test framework dependency silently.

## Coverage Levels

- `CRITICAL`: public methods, edge cases, error paths, contract tests.
- `ALL`: everything in `CRITICAL` plus private/helper methods where practical.
- `NONE`: no tests even if parent flag is true.

## Contract Tests

If `PRECONDITIONS`, `POSTCONDITIONS`, or `INVARIANTS` appear, generate tests for them first.

## Test File Naming

Use language-standard names unless the blueprint specifies otherwise. Respect file-layout
ownership rules from `references/core-language.md`.

## Reporting

List every generated test file in the Implementation Report. Note any test dependency or
framework assumption.
