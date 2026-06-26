# Optional Feature: Docstrings

Run only when `GENERATE_DOCSTRINGS = TRUE`.

Blueprint comments are always preserved. Docstrings are supplementary and must not replace
or reinterpret blueprint comments.

## Header Fields

| Field | Values | Default |
|---|---|---|
| `GENERATE_DOCSTRINGS` | `TRUE`, `FALSE` | `FALSE` |
| `DOCSTRING_STYLE` | style name | auto-detected only when enabled |
| `DOCSTRING_COVERAGE` | `NONE`, `PUBLIC_ONLY`, `ALL` | `PUBLIC_ONLY` |

## Default Styles

| Language | Style |
|---|---|
| Python | Google-style docstrings |
| C# | XML documentation comments |
| Java | Javadoc |
| Kotlin | KDoc |
| C/C++ | Doxygen |
| JavaScript/TypeScript | JSDoc |
| Swift | SwiftDoc / Markdown comments |
| PHP | PHPDoc |
| Rust | rustdoc |
| VBA | manual apostrophe comment blocks |

## Coverage

- `PUBLIC_ONLY`: public/exported classes, methods, functions, properties, interfaces.
- `ALL`: public plus private/protected/internal/helper items.
- `NONE`: no docstrings.

For `embedded_rtos` and `VBA`, cap at `PUBLIC_ONLY` unless the blueprint explicitly requests
more.

## Minimum Content

- Class/Struct: summary.
- Function/Method: summary, parameters, return value, exceptions/errors when relevant.
- Property: summary.
- Enum: enum summary and value summaries.
- Interface/Delegate: contract summary and member descriptions.

## Reporting

Docstring generation does not require a separate report row unless coverage was capped or a
style fallback was applied.
