# Optional Feature: Dependency Graph

Run only when `GENERATE_DEPENDENCY_GRAPH = TRUE`.

## Header Fields

| Field | Values | Default |
|---|---|---|
| `GENERATE_DEPENDENCY_GRAPH` | `TRUE`, `FALSE` | `FALSE` |
| `DEPENDENCY_GRAPH_FORMAT` | `mermaid`, `dot`, `plantuml`, `text` | `mermaid` |
| `FAIL_ON_CIRCULAR_DEPENDENCY` | `TRUE`, `FALSE` | `TRUE` |

## Graph Scope

The dependency graph is a module/class dependency graph, not a function call graph.
Do not mix module dependencies with function reachability.

Edges come from:

1. explicit `USES` statements;
2. explicit `DEPENDS_ON` relationships;
3. cross-module `CALL` statements where the target module/class is known;
4. inheritance or interface implementation dependencies when relevant.

## Algorithm

1. Parse modules/classes and explicit dependencies.
2. Add resolvable cross-module implicit edges.
3. Detect cycles with depth-first search.
4. If cycles exist and `FAIL_ON_CIRCULAR_DEPENDENCY = TRUE`, halt before code generation and
   report the cycles.
5. If cycles exist and `FAIL_ON_CIRCULAR_DEPENDENCY = FALSE`, emit warnings and proceed.
6. Topologically sort the graph when acyclic.
7. Output the graph before source files.

## Cycle Resolution Suggestions

Suggestions may include:

- extract shared code to a new module;
- introduce an interface;
- invert the dependency;
- move event/callback wiring to an outer composition root.

These are suggestions only. Do not rewrite the architecture unless the engineer approves.

## Formats

Default Mermaid example:

```text
graph TD
  A --> B
  B --> C
```

## Reporting

Include graph warnings, cycles, and dependency assumptions in the Implementation Report.
