# PseudoSmith

**Turn structured pseudo-code into production-ready, idiomatic source.**

PseudoSmith is an AI skill that translates **UAB blueprints** — a hybrid of rigid structural
anchors and free-form natural language — into clean, idiomatic code for the language and
platform you specify. It is an **interpreter of intent, not a compiler**. You describe *what*
you want with as much or as little structure as you like; PseudoSmith bridges the gap between
intent and working code.

---

## What is UAB?

**UAB (Universal Architectural Blueprint)** is the input format PseudoSmith reads. It is
*not* a formal programming language — it is a lightweight frame for an engineer to think and
work in. A blueprint mixes two things:

- **Structural anchors** — capitalized, recognizable tokens (`FUNCTION`, `CLASS`, `IF`,
  `BUTTON`, `DATABASE.QUERY`, …) that give the AI clear guardrails and structure.
- **Natural language** — plain English describing behavior ("remove spaces and hyphens",
  "reverse the string", "show an error if the length is wrong").

Anchors are **recommendations, not requirements.** They exist to help *both* the engineer and
the AI stay aligned. You can lean on them heavily for precision, or barely at all and let
natural language carry the intent. PseudoSmith interprets the combination.

```text
/*
 TARGET_LANGUAGE     : Python
 TARGET_PLATFORM     : linux_x86
 TARGET_UI_FRAMEWORK : Tkinter
 DELIVERY_MODE       : inline
*/

MODULE "CCValidator"
  PROCEDURE validateCardNumber(cardNumberString: STRING) -> BOOLEAN
    START
        // Remove spaces and hyphens, then reverse the string
        cleanString    = remove spaces and hyphens from cardNumberString
        reversedDigits = reverse the string cleanString
        ...
    END
END MODULE
```

---

## The UAB header

Every blueprint begins with a **header** enclosed in a `/* ... */` block. The header is the
*control plane* for translation — it tells PseudoSmith what to build and how. The body
describes the logic; the header decides the target.

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

### Required fields

| Field | Description |
|---|---|
| `TARGET_LANGUAGE` | Output language — `Python`, `C`, `C++`, `C#`, `Java`, `Kotlin`, `Swift`, `JavaScript`, `TypeScript`, `VBA`, `PHP`, `Rust`, … (`JavaScript` and `TypeScript` are distinct targets). |
| `TARGET_PLATFORM` | Target OS/architecture — `win32`, `win64`, `linux_x86`, `linux_arm`, `mac_x64`, `mac_arm`, `android_arm64`, `ios_arm64`, `embedded_rtos`, `wasm`, `custom:<name>`, … |

### Optional core fields

| Field | Description |
|---|---|
| `TARGET_LANGUAGE_VERSION` | Version specifier, range, `latest`, or `current`. Falls back to the platform default if omitted. |
| `MEMORY_MODEL` | `garbage_collected`, `manual_ownership`, `automatic_ref_counting`. Inferred from the language if omitted. |
| `CONCURRENCY_MODEL` | `async_await`, `pthreads`, `std_thread`, `coroutines`, `single_threaded`, `rtos_tasks`. |
| `STYLE_GUIDE` | Path or named style guide; overrides inline defaults. |
| `NAMING_CONVENTIONS` | `PascalCase`, `camelCase`, `snake_case`, `kebab-case`, `language_default`. |
| `INDENTATION` | `spaces:4`, `spaces:2`, `tabs` (default `spaces:4`). |
| `DELIVERY_MODE` | `archive` (default), `inline`, or `both`. |

### Conditional fields

These become **required** when the blueprint implies them, and PseudoSmith will pause and ask
rather than choosing silently:

| Field | Becomes required when… |
|---|---|
| `DATABASE_PROVIDER` | The blueprint uses `DATABASE.*` anchors or describes persistence. Values: `sqlite`, `postgresql`, `mysql`, `mariadb`, `sqlserver`, `oracle`, `odbc`, `custom:<name>`. |
| `DATABASE_ACCESS` / `DATABASE_DRIVER` | Database access pattern / a non-standard driver is needed. |
| `TARGET_UI_FRAMEWORK` | GUI anchors appear. `none` (default), `WinForms`, `WPF`, `MAUI`, `Avalonia`, `SwiftUI`, `UIKit`, `AppKit`, `Swing`, `JavaFX`, `AndroidCompose`, `React`, `Qt`, `GTK`, … |

### Optional feature flags (opt-in)

Off by default — set the parent flag to `TRUE` to enable:

| Field | Purpose |
|---|---|
| `GENERATE_UNIT_TESTS` | Emit unit tests (`TEST_FRAMEWORK`, `TEST_COVERAGE`, `TEST_OUTPUT_DIR`). |
| `GENERATE_DOCSTRINGS` | Emit docstrings (`DOCSTRING_STYLE`, `DOCSTRING_COVERAGE`). |
| `GENERATE_DEPENDENCY_GRAPH` | Emit a dependency graph (`DEPENDENCY_GRAPH_FORMAT`, `FAIL_ON_CIRCULAR_DEPENDENCY`). |
| `DETECT_DEAD_CODE` | Report unreachable/unused code. |

> **Value lists are extensible.** Unlisted or `custom:<name>` values are accepted whenever
> PseudoSmith can reasonably generate for the target. If a value materially affects
> implementation and there isn't enough information to proceed safely, it pauses and asks.

---

## Anchors vs. natural language: three styles

Anchors are *optional*. The same program can be expressed anywhere on a spectrum from almost
pure structure to almost pure prose — PseudoSmith interprets the mix. Below, the **same
feature** (validate a credit-card number) is written three ways.

### 1. Anchor-heavy — maximum structure

Best when you want tight control over shape, types, and flow.

```text
/* TARGET_LANGUAGE: Python   TARGET_PLATFORM: linux_x86 */

MODULE "CCValidator"
  PROCEDURE validateCardNumber(cardNumber: STRING) -> BOOLEAN
    START
        clean: STRING = STRIP(cardNumber, " -")
        IF LENGTH(clean) < 13 OR LENGTH(clean) > 19 THEN
            RETURN FALSE
        ENDIF

        sum: INT   = 0
        alt: BOOL  = FALSE
        FOR digit IN REVERSE(clean)
            d: INT = TO_INT(digit)
            IF alt THEN
                d = d * 2
                IF d > 9 THEN d = d - 9 ENDIF
            ENDIF
            sum = sum + d
            alt = NOT alt
        ENDFOR

        RETURN (sum MOD 10) == 0
    END
END MODULE
```

### 2. Balanced — anchors for shape, prose for behavior

The typical sweet spot: anchors fix the skeleton, natural language fills the steps.

```text
/* TARGET_LANGUAGE: Python   TARGET_PLATFORM: linux_x86 */

MODULE "CCValidator"
  PROCEDURE validateCardNumber(cardNumber: STRING) -> BOOLEAN
    START
        // strip spaces and hyphens; reject if not 13-19 digits
        clean = remove all spaces and hyphens from cardNumber
        IF clean is not between 13 and 19 digits THEN
            RETURN FALSE
        ENDIF

        // standard Luhn checksum over the digits
        RETURN clean passes the Luhn check
    END
END MODULE
```

### 3. Prose-heavy — minimal structure

Best for quick intent capture; PseudoSmith infers the structure.

```text
/* TARGET_LANGUAGE: Python   TARGET_PLATFORM: linux_x86 */

MODULE "CCValidator"

Write a function validateCardNumber that takes a card number string and returns true or
false. Ignore spaces and hyphens. Reject anything that isn't 13 to 19 digits long. Otherwise
return whether it passes the standard Luhn checksum.
```

All three produce equivalent, idiomatic output. The difference is **how much you decide
explicitly vs. how much you delegate** — and the more you leave to prose, the more likely a
material gap is surfaced at the ambiguity checkpoint.

---

## Core principles

- **Interpreter of intent, not a compiler.** PseudoSmith realizes your design; it does not
  invent architecture, security policy, frameworks, or product scope on your behalf.
- **Scope fidelity.** It fully realizes what you asked for — inferring routine helpers, error
  handling, glue code, and platform-native equivalents — but it will **not** silently add
  unrequested features, external services, auth flows, storage engines, CI/CD, or UI screens.
- **Material ambiguity is surfaced, not guessed.** When a missing detail would materially
  change architecture, persistence, security, dependencies, public interfaces, UI framework,
  or platform compatibility, PseudoSmith pauses and asks rather than choosing for you.
- **Security is engineer-owned.** It translates the security mechanism you specify; it never
  substitutes a different one silently. Under-specified security-sensitive behavior is flagged.
- **Every translation produces an Implementation Report** documenting decisions, inferences,
  substitutions, risks, and TODOs — so nothing material happens invisibly.

---

## How it works

1. **Header check** — confirms the required header fields (`TARGET_LANGUAGE`,
   `TARGET_PLATFORM`) and any conditionally-required fields (e.g. `DATABASE_PROVIDER`,
   `TARGET_UI_FRAMEWORK`).
2. **Material ambiguity checkpoint** — runs **once per session**; raises only genuinely
   architecture-changing gaps, then proceeds.
3. **Translate** — generates idiomatic source for the target language/platform, preserving
   your comments and respecting your declared dependencies.
4. **Platform compatibility audit** — swaps non-native APIs for platform-correct equivalents
   and records the substitution.
5. **Implementation Report** — delivered as a distinct artefact alongside the code.

Optional, opt-in features: unit tests, docstrings, dependency graphs, and dead-code detection
— each enabled only via its header flag.

---

## Supported targets

**Languages:** Python, C, C++, C#, Java, Kotlin, Swift, JavaScript, TypeScript, VBA, PHP,
Rust *(and others — the value lists are extensible)*.

**UI frameworks:** Tkinter, WinForms, WPF, Avalonia, MAUI, SwiftUI, UIKit, AppKit, Swing,
JavaFX, Android Compose, React, Qt, GTK, and more.

**Databases:** SQLite, PostgreSQL, MySQL/MariaDB, SQL Server, Oracle, ODBC, and custom
providers.

See the `references/platforms/` directory for per-platform mapping rules.

---

## Examples

The `examples/` directory contains complete, worked end-to-end runs. Each scenario pairs a
conforming blueprint with its generated output and Implementation Report:

```text
examples/<scenario>/blueprint.uab.md   — input (header + body)
examples/<scenario>/code.* | code/     — generated output
examples/<scenario>/report.md          — material decisions
```

| Scenario | Targets |
|---|---|
| `ccvalidator` | python-tkinter, vba-libreoffice, csharp-avalonia |

---

## Engineer responsibilities — please read

PseudoSmith generates code from your intent, but it does not replace your judgment. Like any
dependency, linter, or codegen tool you install, you are responsible for understanding how it
behaves before relying on its output.

Before you use this skill:

- **Read the reference files** (`references/`) so you understand how anchors map to your
  target language. Anchors are recommendations; their *interpretation* is documented.
- **Review the Implementation Report for every translation.** It surfaces every material
  decision, inference, substitution, risk, and TODO. If you read nothing else, read this.
- **You own architecture, security, and scope.** PseudoSmith deliberately refuses to make
  those calls silently — that responsibility stays with you.

It is your responsibility to understand the skill you install and use.

---

## Mapping nuances

UAB anchors are deliberately language-agnostic, but the languages they target are not. The
same UAB construct can map to subtly different semantics, idioms, or fallbacks depending on
the target — and PseudoSmith generally maps to the *closest available* construct rather than
inventing behavior to paper over the difference.

A few categories worth understanding before you rely on a mapping:

- **Equality and identity** — e.g. `===` maps to `is` (identity) in Python but `===` in
  JavaScript; these are not the same operation. Choose the operator that matches your intent.
- **Type modeling** — `STRUCT`, `RECORD`, and `ENUM` map to different constructs (class,
  record, dataclass, struct) depending on language and version support.
- **Concurrency** — `ASYNC`/`AWAIT`/`YIELD` map to native idioms only where the target
  language/version supports them, with documented fallbacks where they do not.
- **Version-gated features** — some mappings depend on `TARGET_LANGUAGE_VERSION`; older
  targets receive fallbacks, which are recorded in the report.

In every case, PseudoSmith applies the mapping and **flags material choices in the
Implementation Report.** The reference files document the exact per-language behavior — read
the mappings that matter for your target, and choose the construct that fits your intent.

---

## A note on determinism

PseudoSmith improves the *quality* and *consistency* of generated code, but it does not make
generation deterministic. Because translation is performed by a large language model, running
the **same blueprint** more than once — or across **different AI models** — can still produce
different output. The structure, naming, or internal approach may vary even when the behavior
is equivalent.

You can **minimise** this variance by leaning more on anchors: the tighter and more explicit
your structural anchors and type hints, the less is left to interpretation, and the more
stable the output becomes from run to run. Prose-heavy blueprints give the model more latitude
and therefore vary more.

Treat generated code as a starting point to be reviewed, not a byte-for-byte reproducible
artifact.

---

## License

Copyright © 2026 Jason Mitchell.

PseudoSmith is licensed under the **GNU Affero General Public License v3.0 (AGPLv3)**.
You may use, modify, and redistribute it under the terms of that license. Notably, if you run
a modified version as a network service, you must make your modified source available to its
users.

This program is free software: you can redistribute it and/or modify it under the terms of the
GNU Affero General Public License as published by the Free Software Foundation, either version
3 of the License, or (at your option) any later version. It is distributed in the hope that it
will be useful, but **WITHOUT ANY WARRANTY**; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

## Contributing

Issues and pull requests welcome. Please keep changes consistent with the normative reference
files — on any conflict between examples and the references, **the references win.**
