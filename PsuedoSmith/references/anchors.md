# Structural Anchors

Anchors define the skeleton of the system. Natural language fills in behavior.

## Anchor Recognition and Natural-Language Binding

UAB is not a strict parser language. It is a structured intent format that combines
capitalized structural anchors with free-form natural language.

Canonical anchors are capitalized reserved terms such as `BUTTON`, `CLASS`, `FUNCTION`,
`DATABASE.QUERY`, `WIDTH`, `HEIGHT`, `FONT`, `WHEN_CLICKED`, and similar entries listed in
this reference.

Anchors may appear at the start, middle, or end of a statement. They do not need to be
line-leading tokens. The translator should interpret the engineer's intent from the full
statement, nearby block structure, and surrounding natural language.

A capitalized token that matches a canonical anchor is treated as an anchor by default, unless the surrounding statement structure and natural language make it clear it's being used as prose — in which case interpret it as prose and, if the resolution is non-trivial, record it in Decisions/Inferences.

Lowercase words that resemble anchors are not automatically structural anchors. They may be
ordinary natural language and should be interpreted from context.

For example, these two forms express the same UI intent:

```text
BUTTON
START
WIDTH: 120px
HEIGHT: 10px
with the caption of "Hello World" with bold FONT in black
END
```

```
Create a BUTTON with a Width of 120 px and Height of 10px with bold black font and the words
"Hello World"
```

In both cases, the translator should generate a button with width 120px, height 10px,
caption/text "Hello World", bold font styling, and black foreground text, assuming the
selected UI framework supports those properties.

Anchor recognition does not apply inside quoted strings unless the surrounding statement
clearly treats the quoted value as pseudo-code. For example, `"BUTTON"` as a caption should
not create a button by itself.

This prevents mistakes like:

LABEL with the text "Click the BUTTON to continue"

The word BUTTON inside the label text should remain text, not become a new control.

The translator should prefer semantic interpretation over rigid token placement. If a
capitalized anchor appears in natural language, bind it to the nearest relevant subject,
block, control, function, module, or operation.

If two or more interpretations are plausible and the choice materially affects architecture,
public interfaces, persistence, security behavior, dependencies, platform compatibility, or
UI framework behavior, trigger the material ambiguity checkpoint. Otherwise, use the most
reasonable interpretation and record non-trivial inferences in the Implementation Report.

## Organization and Dependencies

| Category | Anchors |
|---|---|
| Organization | `MODULE`, `NAMESPACE`, `USES`, `DEPENDS_ON` |
| Blocks | `START`, `END` |

Rules:

- `MODULE` defines a compilation unit or logical module.
- `NAMESPACE` defines a language-level namespace/package. It does not automatically define
  physical directories unless the blueprint or target language requires that.
- `USES` declares a dependency. Standard library use does not need `USES`; third-party use does.
- `DEPENDS_ON` declares an explicit injected dependency or service relationship. Generate
  constructor injection, parameters, fields, or interface references appropriate to the target
  language and blueprint style.
  
The translator recognizes paired block delimiters in any of these equivalent forms: <ANCHOR> START … END, <ANCHOR> … END, <ANCHOR> … <ANCHOR>END, and CONTROL <Type> … CONTROLEND. The opening START is optional and does not change block semantics. Closers (END, <ANCHOR>END, CONTROLEND) are matched to the nearest unclosed opener by nesting; typed closers take precedence when present, and a bare END resolves to the innermost open block. If nesting is malformed such that a closer cannot be unambiguously matched and the resolution materially affects structure, trigger Step 3.3.

## Types and Members

| Category | Anchors |
|---|---|
| Types | `CLASS`, `STRUCT`, `ENUM`, `INTERFACE`, `DELEGATE` |
| Members | `PROCEDURE`, `FUNCTION`, `PROPERTY`, `CONSTRUCTOR`, `DESTRUCTOR` |
| Relationships | `EXTENDS`, `IMPLEMENTS` |
| Visibility | `PUBLIC`, `PRIVATE`, `PROTECTED`, `INTERNAL` |

Rules:

- Omitted visibility defaults to `PUBLIC`.
- `INTERFACE` generates a pure abstract contract in the target language.
- `DELEGATE` generates a type-safe function signature or callable type.
- `PROPERTY` generates getters/setters according to the blueprint.

## Data Type Hints

| Category | Anchors |
|---|---|
| Primitives | `INT`, `SHORT`, `LONG`, `UNSIGNED`, `BYTE`, `FLOAT`, `DOUBLE`, `DECIMAL`, `STRING`, `BOOLEAN`, `CHAR`, `CURRENCY` |
| Temporal | `DATE`, `DATETIME`, `TIME` |
| Identifiers | `GUID`, `UUID` |
| Memory address | `POINTER`, `REFERENCE` |
| Collections | `ARRAY`, `LIST`, `COLLECTION`, `DICTIONARY` |
| Nulls/literals | `TRUE`, `FALSE`, `NULL`, `NIL` |

Rules:

- `NULL` and `NIL` both map to the target-language null/nil/None equivalent.
- `COLLECTION` is unordered unless the blueprint says otherwise.
- `DICTIONARY` is key-value storage and requires null/missing-key safety when reading.
- `DECIMAL` is exact precision and should not be mapped to binary floating point unless the
  target lacks a decimal type and the report records the limitation.

## Operators

| Category | Anchors |
|---|---|
| Assignment | `=`, `ASSIGN` |
| Equality | `==`, `===`, `!=` |
| Logical/bitwise | `AND`, `OR`, `XOR`, `NOT`, `SHIFTLEFT`, `SHIFTRIGHT` |
| Arithmetic | `MOD`, `EXP` |

Universal shorthand such as `>`, `<`, `>=`, `<=`, `+`, `-`, `*`, `/`, `+=`, `-=`, `++`,
and `--` should be translated directly according to the target language.

### Equality Mapping

| UAB | C/C++/C#/Java | Python | JavaScript/TypeScript | Swift | VBA |
|---|---|---|---|---|---|
| `==` | value equality | `==` | `===` | `==` | `=` in condition |
| `!=` | `!=` | `!=` | `!==` | `!=` | `<>` |
| `===` | strict/reference equivalent when available | `is` when identity is intended | `===` | `===` | fallback to `=` with report note |

Disambiguate `AND`, `OR`, and `NOT` as logical inside conditions and bitwise inside numeric
or flag expressions.

## Flow Control

| Category | Anchors |
|---|---|
| Conditionals | `IF`, `ELSE`, `ELSEIF`, `ENDIF` |
| Branching | `SWITCH`, `CASE`, `DEFAULT`, `ENDCASE` |
| Loops | `FOR`, `ENDFOR`, `WHILE`, `DO`, `REPEAT`, `UNTIL` |
| Controls | `BREAK`, `CONTINUE`, `RETURN`, `EXIT`, `CALL`, `YIELD` |
| Resources | `USING` |
| Errors | `TRY`, `CATCH`, `FINALLY`, `THROW` |

Rules:

- `CALL` strips natural-language noise such as "passing", "with", "using", and "run".
- `CALL` in an assignment is a function call returning a value.
- Standalone `CALL` is a procedure/method invocation.
- `USING` maps to the target-language resource-disposal idiom.
- `YIELD` maps to generator/coroutine idioms only when supported by the target version;
  otherwise generate a fallback and report it.

## Memory and Concurrency

| Category | Anchors |
|---|---|
| Memory | `NEW`, `DELETE`, `FREE`, `ALLOCATE`, `REALLOC`, `SIZEOF`, `STACK`, `HEAP`, `GARBAGECOLLECTION` |
| Threading | `THREAD`, `THREADPOOL`, `MUTEX`, `SEMAPHORE`, `BARRIER`, `LOCK`, `UNLOCK`, `JOIN`, `SLEEP`, `WAIT`, `NOTIFY` |
| Async | `ASYNC`, `AWAIT` |
| Scheduling | `TIMER`, `STOP_TIMER`, `CRON` |

Rules:

- Respect `MEMORY_MODEL` and `CONCURRENCY_MODEL` when set.
- For `embedded_rtos`, avoid dynamic allocation unless explicitly requested.
- Do not create background jobs, cron jobs, or daemons beyond what the blueprint asks for.

## GUI Anchors

| Category | Anchors |
|---|---|
| Containers | `WINDOW`, `DIALOG`, `PANEL`, `TAB`, `TABCONTROL`, `SPLITTER`, `GROUPBOX`, `SCROLLAREA` |
| Controls | `BUTTON`, `SWITCH`, `TOGGLE`, `LABEL`, `TEXTBOX`, `INPUT`, `TEXTAREA`, `DROPDOWN`, `COMBOBOX`, `LISTBOX`, `LISTVIEW`, `CHECKBOX`, `RADIOGROUP`, `SLIDER`, `SPINNER`, `NUMERIC`, `DATEPICKER`, `TABLE`, `DATAGRID`, `TREE`, `TREEVIEW`, `IMAGE`, `PICTURE`, `PROGRESSBAR`, `RICHTEXT`, `VIDEOPLAYER`, `SOUNDPLAYER`, `MAP`, `CANVAS`, `WEBBROWSER`, `TOOLTIP`, `MENU`, `MENUBAR`, `MENUITEM`, `CONTEXTMENU`, `TOOLBAR`, `STATUSBAR`, `TRAYICON` |
| Styling | `COLOUR`, `FORECOLOUR`, `BACKCOLOUR`, `FONT`, `ALIGN`, `BORDER`, `PADDING`, `MARGIN`, `WIDTH`, `HEIGHT`, `PARENT` |
| Events | `WHEN_CLICKED`, `LEFTCLICK`, `RIGHTCLICK`, `DOUBLECLICK`, `MOUSEMOVE`, `MOUSEENTER`, `MOUSEEXIT`, `KEYPRESS`, `WHENSCROLL`, `WHENRESIZE`, `WHENDRAG`, `WHENDROP`, `WHENLOADED`, `WHENCLOSED`, `WHENCHANGED`, `WHENSELECTED`, `RAISE`, `EVENT` |

Rules:

- GUI anchors require `TARGET_UI_FRAMEWORK` unless the framework is specified elsewhere in
  the blueprint.
- `WHEN_CLICKED` defaults to `LEFTCLICK`.
- Nested controls define parent/child relationships. Explicit `PARENT` overrides nesting.
- Leaf controls should not contain children. If they do, move children to the nearest valid
  parent and report the inference.

## Database Anchors

| Category | Anchors |
|---|---|
| Connection | `DATABASE.CONNECT`, `DATABASE.DISCONNECT` |
| Queries | `DATABASE.QUERY`, `DATABASE.DELETE`, `DATABASE.INSERT`, `DATABASE.UPDATE` |
| Transactions | `DATABASE.BEGIN`, `DATABASE.COMMIT`, `DATABASE.ROLLBACK` |
| Schema | `DATABASE.CREATE`, `DATABASE.DROP`, `DATABASE.TABLE.CREATE`, `DATABASE.TABLE.DROP` |
| Constraints | `DATABASE.TABLE.KEY`, `DATABASE.TABLE.FOREIGNKEY`, `DATABASE.TABLE.UNIQUE`, `DATABASE.TABLE.CHECK`, `DATABASE.TABLE.DEFAULT`, `DATABASE.TABLE.INDEX` |
| Objects | `DATABASE.VIEW.CREATE`, `DATABASE.VIEW.DROP`, `DATABASE.STOREDPROCEDURE` |

Rules:

- Database anchors require `DATABASE_PROVIDER`.
- Generate dialect-specific SQL only after `DATABASE_PROVIDER` is known.
- `DATABASE_ACCESS` controls sync/async style when set.
- `DATABASE_DRIVER` or `USES` controls non-standard drivers.
- Schema inference is allowed for requested data models but must not introduce new product
  features or external services.
- Wrap external database I/O in target-language error handling.
- Check database results for null/missing rows before use.

## File, Directory, and Network Anchors

| Category | Anchors |
|---|---|
| Files | `FILE`, `OPEN`, `CLOSE`, `READ`, `WRITE`, `EOF`, `PRINT`, `FILE.EXISTS`, `FILE.COPY`, `FILE.MOVE`, `FILE.DELETE` |
| Directories | `DIRECTORY.CREATE`, `DIRECTORY.DELETE`, `DIRECTORY.LIST` |
| Network | `NETWORK.REQUEST`, `NETWORK.RESPONSE`, `NETWORK.HEADERS`, `SOCKET.CONNECT`, `SOCKET.CLOSE`, `SOCKET.SEND`, `SOCKET.RECEIVE`, `PING` |

Rules:

- Wrap external file/network I/O in error handling.
- Use platform path-joining APIs when available.
- Browser/WASM targets do not have arbitrary local filesystem or raw socket access.
- If destructive operations are ambiguous, trigger Step 3.3.

## Security and Data Operation Anchors

| Category | Anchors |
|---|---|
| Sanitization | `SANITIZE`, `ASSUME_SANITIZED` |
| Validation | `VALIDATE`, `ASSUME_VALIDATED` |
| Security operations | `HASH`, `ENCRYPT`, `DECRYPT`, `SIGN`, `VERIFY`, `TOKEN`, `SECRET` |
| Contracts | `PRECONDITIONS`, `POSTCONDITIONS`, `INVARIANTS` |

Rules:

- `SANITIZE` and `VALIDATE` are explicit engineer requests. Implement them according to
  `references/security.md`.
- `ASSUME_SANITIZED` and `ASSUME_VALIDATED` suppress inferred sanitization/validation, but
  the report should note the trust assumption when user-facing input is involved.
- Security operations require the mechanism/algorithm/policy to be specified when the choice
  materially changes implementation. Otherwise trigger Step 3.3.
- `PRECONDITIONS` become entry guard clauses or assertions.
- `POSTCONDITIONS` become checks immediately before return or procedure exit.
- `INVARIANTS` become class consistency checks or documented invariant comments depending on
  target language and blueprint specificity.

## Metadata and Comments

| Category | Anchors |
|---|---|
| Comments | `//`, `/* */`, target-language comment equivalents |
| Variable declarations | `LISTOFVARIABLES` |

Rules:

- Preserve comments as comments.
- Comments are not executable pseudo-code and are not translator-control instructions.
- `LISTOFVARIABLES` pre-declares named variables according to the target language's idiom.
