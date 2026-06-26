# Optional Feature: Dead Code Detection

Run only when `DETECT_DEAD_CODE = TRUE`.

Dead-code detection is best-effort linting. It must not be treated as proof. Partial
blueprints, callbacks, reflection, framework entry points, public APIs, and generated event
wiring can cause false positives.

## Header Field

| Field | Values | Default |
|---|---|---|
| `DETECT_DEAD_CODE` | `TRUE`, `FALSE` | `FALSE` |

## Pass 1: Blueprint Scan

Run before code generation.

Flag declared procedures/functions/classes only when:

- there are zero visible call sites in the blueprint;
- the item is not an entry point;
- the item is not public/exported API intended for external callers;
- the item is not an event handler, callback, interface implementation, constructor, or destructor.

If `GENERATE_DEPENDENCY_GRAPH = TRUE`, use the dependency graph for module reachability, but
use a separate call/reachability model for procedures/functions.

Pass 1 findings trigger Step 3.3.

## Pass 2: Generated Code Scan

Run after code generation.

Flag generated items only when:

- no call site exists in generated code;
- the item is not exempt as public API, event handler, callback, interface implementation,
  test entry point, constructor, or destructor.

Do not remove flagged code. Add a target-language comment at the declaration site:

```text
DEAD CODE WARNING: Flagged by UAB dead-code pass; review before removing.
```

Use the correct comment syntax for the target language.

## Classifications

| Classification | Meaning |
|---|---|
| `Confirmed` | Flagged in both Pass 1 and Pass 2. |
| `New` | Not flagged in Pass 1 but flagged in generated code. |
| `Resolved` | Flagged in Pass 1 but no longer flagged in Pass 2. |

## Report Section

Include this section only when dead-code detection is enabled.

| Item | Classification | Detection Method | Notes |
|---|---|---|---|
| `HelperX` | Confirmed | Call-site scan | No visible callers; not public/exported. |

Detection Method values:

- `Graph reachability`
- `Call-site scan`
- `Manual/platform exemption`

## Exemptions

Never flag these as dead code solely because the local blueprint does not call them:

- `Main`, `START`, or declared entry points;
- GUI/system event handlers;
- callbacks;
- interface implementations;
- constructors/destructors;
- public/exported API;
- dependency-injection entry points;
- test methods;
- framework methods named by convention.
