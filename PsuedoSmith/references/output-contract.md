# Output Contract

## Delivery Policy

Before translation, ask whether the user wants source output inline in chat or as
downloadable files.

Recommend downloadable files when any of these are true:

- multiple modules/classes are generated;
- tests are enabled;
- dependency graph generation is enabled;
- database DDL/migrations are generated;
- output is long enough that inline code would be hard to use.

The Implementation Report is always distinct:

- downloadable mode: generate a separate `.md` report file;
- inline mode: output the report as a separate Markdown section after code blocks.

Never mix the report into source-code blocks.

## Artefact Ordering

Present generated artefacts in this order:

1. dependency graph, if enabled;
2. source files;
3. database files or migration files, if generated;
4. test files, if enabled;
5. documentation/docstring-related artefacts, if any;
6. Implementation Report last.

The `logs/` directory and `logs/translation-state.md` are excluded from this ordering; they
are infrastructure, not presented artefacts.

## Implementation Report

The Implementation Report is assembled from the internal Translation Log.

The report filename should be `<BlueprintOrModuleName>_report.md` when generating files.
If the blueprint name is unknown, use `PseudoSmith_translation_report.md`.

## Report Sections

### 1. Translation Summary

Include:

- blueprint/module name;
- `TARGET_LANGUAGE`;
- `TARGET_PLATFORM`;
- `TARGET_LANGUAGE_VERSION`, if provided;
- skill version used: `v0.9`;
- overall status: `Fully Implemented`, `Partially Implemented`, or `Could Not Be Implemented`;
- timestamp in UTC;
- short blueprint fingerprint based on the header or first meaningful blueprint lines.

### 2. Implementation Status

This section is the engineer-facing projection of internal session state. It is **derived
from** the Translation Plan and Translation Log in `logs/translation-state.md` and must
remain consistent with them; it is not maintained independently. `logs/translation-state.md`
itself is workflow infrastructure and is never reported as an artefact.

One row per declared `MODULE`, `CLASS`, `PROCEDURE`, `FUNCTION`, and generated artefact of
interest.

| Item | Type | Status | Notes |
|---|---|---|---|
| `ValidateCardNumber` | FUNCTION | Fully Implemented | Direct mapping from blueprint. |
| `UserRepository` | CLASS | Partially Implemented | Database driver missing; TODO emitted. |
(Rows above are examples; replace with the actual items from the Plan.)

Status values (delivery outcome -- distinct from the Plan's lifecycle states):

Fully Implemented -- item translated with no unresolved gaps.
Partially Implemented -- item translated but carries a TODO or unresolved ambiguity.
Not Implemented -- item could not be translated.

Mapping from internal state: a Plan item is eligible for this section once its Plan status is
completed or blocked. Translate to delivery status using the Log:

Plan completed + no TODO/risk in Log -> Fully Implemented;
Plan completed + TODO or unresolved item in Log -> Partially Implemented;
Plan blocked, or item omitted from output -> Not Implemented.

### 3. Decisions and Inferences

Numbered list of non-trivial decisions made by the translator.

Format:

```text
1. **[Subject]** -- [Decision]. Reason: [Why].
```

Do not list mechanical keyword mappings.

### 4. Confidence and Risk

Include items with less than High confidence or explicit risk.

| Item | Confidence | Risk / Note |
|---|---|---|
| `PasswordHash` | Medium | Mechanism implemented exactly as blueprint requested; engineer should review. |
| `ReportPath` | Low | Path policy was not specified; flat output used. |

Confidence levels:

- `High` -- direct mapping from explicit blueprint/header.
- `Medium` -- required inference or platform substitution.
- `Low` -- significant inference, unavailable platform equivalent, or manual review needed.

Always report:

- user-facing input with no explicit `SANITIZE` or `VALIDATE` when the blueprint appears to
  use that input in a sensitive context;
- security-sensitive behavior implemented exactly as specified but considered risky;
- missing `DATABASE_DRIVER` or dependency-driven TODOs;
- GUI framework substitutions;
- platform API fallbacks;
- language-version fallbacks;
- file-layout assumptions.

### 5. Dead Code

Include only when `DETECT_DEAD_CODE = TRUE`.

Follow `references/optional-features/dead-code.md`.

### 6. TODO Items

List every TODO emitted in generated code.

| File | Location | Description |
|---|---|---|
| `user_repository.py` | `connect` | Add declared PostgreSQL driver before use. |

If none, write `None`.

### 7. Generated Artefacts

Complete inventory of files or inline artefacts produced.

| File | Type | Description |
|---|---|---|
| `user_repository.py` | Source | Repository implementation. |
| `schema.sql` | Database | Generated DDL. |

Do not list workflow-infrastructure files in this inventory. Specifically,
`logs/translation-state.md` is internal session state used for resumption, not a deliverable,
and must never appear in Generated Artefacts, Artefact Ordering, or the Implementation Report.

## Failure Handling

If translation cannot proceed because a required field or material choice is missing, do not
generate partial code unless the engineer explicitly chooses to proceed from Step 3.3.

If proceeding with unresolved material ambiguity, emit TODO comments and mark affected items
as `Partially Implemented` or `Not Implemented` as appropriate.
