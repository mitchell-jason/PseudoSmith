# Anchors Reference

This document lists every recognized UAB anchor and its intended use.

**Anchors are recommendations, not requirements.** They exist to help the engineer and the AI
stay aligned. A blueprint may use them heavily for precision or barely at all and let natural
language carry the intent — PseudoSmith interprets the combination. An unlisted capitalized
token is acceptable whenever its meaning is reasonably clear; the lists below are extensible,
not exhaustive.

---

## How anchors are recognized

- **Canonical anchors are capitalized** reserved terms (`BUTTON`, `CLASS`, `FUNCTION`,
  `DATABASE.QUERY`, `WIDTH`, `WHEN_CLICKED`, …).
- Anchors may appear at the **start, middle, or end** of a statement — they need not be
  line-leading tokens. Intent is read from the whole statement, the surrounding block, and
  nearby natural language.
- A capitalized token matching a canonical anchor is treated as an anchor **by default**,
  unless the statement structure and prose make clear it is being used as ordinary language —
  in which case it is interpreted as prose, and any non-trivial resolution is recorded in the
  Implementation Report.
- **Lowercase** look-alikes are not automatically anchors; they are interpreted from context.
- Anchor recognition **does not apply inside quoted strings** unless the surrounding statement
  clearly treats the quoted value as pseudo-code. A `LABEL` with text `"Click the BUTTON"`
  does not create a button.
- When two interpretations are plausible **and** the choice materially affects architecture,
  interfaces, persistence, security, dependencies, platform compatibility, or UI behavior, the
  material-ambiguity checkpoint is triggered. Otherwise the most reasonable interpretation is
  used and recorded.

### Block delimiters

Blocks are opened and closed flexibly. All of the following are recognized as equivalent ways
to bound an `<ANCHOR>` block:

```text
<ANCHOR> START … END          // explicit START opener (START is optional, never a closer)
<ANCHOR> … <ANCHOR>END         // matched-name closer, e.g. BUTTON … BUTTONEND
<ANCHOR> START … <ANCHOR>END   // combination of the above
```

`START` is an **optional opener** delimiter and is never itself a closer. A closer may be the
generic `END`, the matched-name form (`BUTTONEND`, `FUNCTIONEND`, …), or a category keyword
(`ENDIF`, `ENDFOR`, `ENDCASE`). Where a bare `END` is ambiguous, it binds to the nearest open
block; if that is genuinely unclear and material, the ambiguity checkpoint is triggered.

---

## Organization and Dependencies

| Anchor | Intended use |
|---|---|
| `MODULE` | Defines a compilation unit or logical module. |
| `NAMESPACE` | Defines a language-level namespace/package. Does **not** imply physical directories unless the blueprint or target language requires it. |
| `USES` | Declares a dependency. Standard-library use needs no `USES`; third-party use does. |
| `DEPENDS_ON` | Declares an explicit injected dependency/service relationship — generates constructor injection, parameters, fields, or interface references as fits the target. |
| `START` / `END` | Generic block open/close delimiters (see *Block delimiters* above). |

---

## Types and Members

| Anchor | Intended use |
|---|---|
| `CLASS` | Defines a class. |
| `STRUCT` | Defines a value/record-style type (maps to struct, record, or dataclass per language). |
| `ENUM` | Defines an enumeration. |
| `INTERFACE` | Generates a pure abstract contract in the target language. |
| `DELEGATE` | Generates a type-safe function signature or callable type. |
| `PROCEDURE` | A routine with no required return value. |
| `FUNCTION` | A routine that returns a value. |
| `PROPERTY` | Generates getters/setters per the blueprint. |
| `CONSTRUCTOR` / `DESTRUCTOR` | Type initialization / finalization. |
| `EXTENDS` | Inheritance relationship. |
| `IMPLEMENTS` | Interface-implementation relationship. |
| `PUBLIC` / `PRIVATE` / `PROTECTED` / `INTERNAL` | Visibility. Omitted visibility defaults to `PUBLIC`. |

---

## Data Type Hints

| Anchor | Intended use |
|---|---|
| `INT`, `SHORT`, `LONG`, `UNSIGNED`, `BYTE` | Integer family. |
| `FLOAT`, `DOUBLE` | Binary floating point. |
| `DECIMAL` | **Exact** precision; not mapped to binary float unless the target lacks a decimal type (limitation recorded in the report). |
| `STRING`, `CHAR` | Text / single character. |
| `BOOLEAN` | Boolean. |
| `CURRENCY` | Monetary value (prefer exact precision). |
| `DATE`, `DATETIME`, `TIME` | Temporal types. |
| `GUID`, `UUID` | Unique identifiers. |
| `POINTER`, `REFERENCE` | Memory address / reference types. |
| `ARRAY`, `LIST` | Ordered collections. |
| `COLLECTION` | Unordered unless the blueprint says otherwise. |
| `DICTIONARY` | Key-value storage; requires null/missing-key safety on read. |
| `TRUE`, `FALSE` | Boolean literals. |
| `NULL`, `NIL` | Both map to the target's null/nil/None equivalent. |

---

## Operators

| Anchor | Intended use |
|---|---|
| `=`, `ASSIGN` | Assignment. |
| `==` | Value equality. |
| `===` | Strict/identity equality — **language-dependent**; see note below. |
| `!=` | Inequality. |
| `AND`, `OR`, `XOR`, `NOT` | Logical inside conditions; bitwise inside numeric/flag expressions. |
| `SHIFTLEFT`, `SHIFTRIGHT` | Bit shifts. |
| `MOD` | Modulo. |
| `EXP` | Exponentiation. |

Universal shorthand — `>`, `<`, `>=`, `<=`, `+`, `-`, `*`, `/`, `+=`, `-=`, `++`, `--` — is
translated directly to the target language.

### Equality mapping

| UAB | C / C++ / C# / Java | Python | JS / TS | Swift | VBA |
|---|---|---|---|---|---|
| `==` | value equality | `==` | `===` | `==` | `=` in condition |
| `!=` | `!=` | `!=` | `!==` | `!=` | `<>` |
| `===` | strict/reference equivalent when available | `is` (identity) | `===` | `===` | `=` with report note |

> **Note on `===`.** Its meaning differs across targets: in JavaScript it is strict value
> equality, while in Python it maps to `is` (object **identity**), which is not the same
> operation and can behave unexpectedly with interned values. Choose the operator that matches
> your intent; whenever `===` is translated to an identity check, the choice is flagged in the
> Implementation Report.

---

## Flow Control

| Anchor | Intended use |
|---|---|
| `IF`, `ELSEIF`, `ELSE`, `ENDIF` | Conditionals. |
| `SWITCH`, `CASE`, `DEFAULT`, `ENDCASE` | Multi-way branching. |
| `FOR`, `ENDFOR`, `WHILE`, `DO`, `REPEAT`, `UNTIL` | Loops. |
| `BREAK`, `CONTINUE` | Loop control. |
| `RETURN` | Return from a routine. |
| `EXIT` | Exit the current construct/program per context. |
| `CALL` | Invoke a routine; strips noise words ("passing", "with", "using", "run"). In an assignment it is a value-returning call; standalone it is a procedure/method invocation. |
| `YIELD` | Generator/coroutine production where supported; otherwise a reported fallback. |
| `USING` | Maps to the target's resource-disposal idiom. |
| `TRY`, `CATCH`, `FINALLY`, `THROW` | Error handling. |

---

## Memory and Concurrency

| Anchor | Intended use |
|---|---|
| `NEW`, `DELETE`, `FREE`, `ALLOCATE`, `REALLOC` | Allocation / deallocation. |
| `SIZEOF` | Size query. |
| `STACK`, `HEAP` | Storage-location hints. |
| `GARBAGECOLLECTION` | GC interaction where applicable. |
| `THREAD`, `THREADPOOL` | Thread / pool creation. |
| `MUTEX`, `SEMAPHORE`, `BARRIER`, `LOCK`, `UNLOCK` | Synchronization primitives. |
| `JOIN`, `SLEEP`, `WAIT`, `NOTIFY` | Thread coordination. |
| `ASYNC`, `AWAIT` | Async idioms where supported by the target version. |
| `TIMER`, `STOP_TIMER`, `CRON` | Scheduling. |

Rules: respect `MEMORY_MODEL` and `CONCURRENCY_MODEL` when set; for `embedded_rtos` avoid
dynamic allocation unless explicitly requested; do not create background jobs, cron jobs, or
daemons beyond what the blueprint asks for.

---

## GUI

GUI anchors require `TARGET_UI_FRAMEWORK` unless the framework is otherwise specified.
Nested controls define parent/child relationships; explicit `PARENT` overrides nesting. Leaf
controls should not contain children (children are moved to the nearest valid parent and the
inference reported).

| Category | Anchors |
|---|---|
| Containers | `WINDOW`, `DIALOG`, `PANEL`, `TAB`, `TABCONTROL`, `SPLITTER`, `GROUPBOX`, `SCROLLAREA` |
| Controls | `BUTTON`, `SWITCH`, `TOGGLE`, `LABEL`, `TEXTBOX`, `INPUT`, `TEXTAREA`, `DROPDOWN`, `COMBOBOX`, `LISTBOX`, `LISTVIEW`, `CHECKBOX`, `RADIOGROUP`, `SLIDER`, `SPINNER`, `NUMERIC`, `DATEPICKER`, `TABLE`, `DATAGRID`, `TREE`, `TREEVIEW`, `IMAGE`, `PICTURE`, `PROGRESSBAR`, `RICHTEXT`, `VIDEOPLAYER`, `SOUNDPLAYER`, `MAP`, `CANVAS`, `WEBBROWSER`, `TOOLTIP`, `MENU`, `MENUBAR`, `MENUITEM`, `CONTEXTMENU`, `TOOLBAR`, `STATUSBAR`, `TRAYICON` |
| Styling | `COLOUR`, `FORECOLOUR`, `BACKCOLOUR`, `FONT`, `ALIGN`, `BORDER`, `PADDING`, `MARGIN`, `WIDTH`, `HEIGHT`, `PARENT` |
| Events | `WHEN_CLICKED` (defaults to `LEFTCLICK`), `LEFTCLICK`, `RIGHTCLICK`, `DOUBLECLICK`, `MOUSEMOVE`, `MOUSEENTER`, `MOUSEEXIT`, `KEYPRESS`, `WHENSCROLL`, `WHENRESIZE`, `WHENDRAG`, `WHENDROP`, `WHENLOADED`, `WHENCLOSED`, `WHENCHANGED`, `WHENSELECTED`, `RAISE`, `EVENT` |

---

## Database

Database anchors require `DATABASE_PROVIDER`; dialect-specific SQL is generated only once the
provider is known. `DATABASE_ACCESS` controls sync/async style; `DATABASE_DRIVER` or `USES`
controls non-standard drivers. Schema inference is allowed for the requested data model but
must not introduce new product features or external services. External DB I/O is wrapped in
error handling, and results are checked for null/missing rows before use.

| Category | Anchors |
|---|---|
| Connection | `DATABASE.CONNECT`, `DATABASE.DISCONNECT` |
| Queries | `DATABASE.QUERY`, `DATABASE.INSERT`, `DATABASE.UPDATE`, `DATABASE.DELETE` |
| Transactions | `DATABASE.BEGIN`, `DATABASE.COMMIT`, `DATABASE.ROLLBACK` |
| Schema | `DATABASE.CREATE`, `DATABASE.DROP`, `DATABASE.TABLE.CREATE`, `DATABASE.TABLE.DROP` |
| Constraints | `DATABASE.TABLE.KEY`, `DATABASE.TABLE.FOREIGNKEY`, `DATABASE.TABLE.UNIQUE`, `DATABASE.TABLE.CHECK`, `DATABASE.TABLE.DEFAULT`, `DATABASE.TABLE.INDEX` |
| Objects | `DATABASE.VIEW.CREATE`, `DATABASE.VIEW.DROP`, `DATABASE.STOREDPROCEDURE` |

---

## File, Directory, and Network

External file/network I/O is wrapped in error handling; platform path-joining APIs are used
where available. Browser/WASM targets have no arbitrary local filesystem or raw socket access.
Ambiguous destructive operations trigger the ambiguity checkpoint.

| Category | Anchors |
|---|---|
| Files | `FILE`, `OPEN`, `CLOSE`, `READ`, `WRITE`, `EOF`, `PRINT`, `FILE.EXISTS`, `FILE.COPY`, `FILE.MOVE`, `FILE.DELETE` |
| Directories | `DIRECTORY.CREATE`, `DIRECTORY.DELETE`, `DIRECTORY.LIST` |
| Network | `NETWORK.REQUEST`, `NETWORK.RESPONSE`, `NETWORK.HEADERS`, `SOCKET.CONNECT`, `SOCKET.CLOSE`, `SOCKET.SEND`, `SOCKET.RECEIVE`, `PING` |

---

## Security and Data Operations

Security behavior is engineer-owned (see `references/security.md`). PseudoSmith implements the
specified mechanism and never substitutes a different one silently.

| Anchor | Intended use |
|---|---|
| `SANITIZE` | Explicit request to sanitize input — implemented per `security.md`. |
| `ASSUME_SANITIZED` | Suppresses inferred sanitization; trust assumption noted when user-facing input is involved. |
| `VALIDATE` | Explicit request to validate input. |
| `ASSUME_VALIDATED` | Suppresses inferred validation; trust assumption noted. |
| `HASH`, `ENCRYPT`, `DECRYPT`, `SIGN`, `VERIFY` | Security operations — require a specified mechanism/algorithm/policy when the choice materially changes implementation; otherwise the ambiguity checkpoint is triggered. |
| `TOKEN`, `SECRET` | Token / secret handling per the specified policy. |
| `PRECONDITIONS` | Entry guard clauses or assertions. |
| `POSTCONDITIONS` | Checks immediately before return/exit. |
| `INVARIANTS` | Class consistency checks or documented invariant comments, per target and specificity. |

---

## Metadata and Comments

| Anchor | Intended use |
|---|---|
| `//`, `/* */` | Comments. Preserved as comments; **not** executable pseudo-code and **not** translator-control instructions. |
| target-language comment equivalents | Recognized and preserved. |
| `LISTOFVARIABLES` | Pre-declares named variables per the target language's idiom. |
