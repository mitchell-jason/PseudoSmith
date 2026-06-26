# Platform Reference: Rust

## Default Version

Default: Rust stable with edition 2021.

Do not create `Cargo.toml` unless the blueprint requests a project/package output or a crate
layout is necessary for downloadable output.

## Types

- `STRING` -> `String` or `&str` depending on ownership.
- `INT` -> `i32` unless size is specified.
- `LONG` -> `i64`.
- `BOOLEAN` -> `bool`.
- `LIST` -> `Vec<T>` on `std` targets.
- `COLLECTION` -> `HashSet<T>` on `std` targets.
- `DICTIONARY` -> `HashMap<K,V>` on `std` targets.
- Use `Result<T, E>` for fallible operations.

## Type, Interface, and Member Mapping

- `INTERFACE` -> `trait`. Rust has no class-based inheritance; model abstract
  contracts as traits and implement them with `impl Trait for Type`.
- `CLASS` / `STRUCT` -> `struct` (with `impl` blocks for methods). Rust has no
  `class`; do not invent inheritance hierarchies. Prefer composition and traits.
- `INHERITANCE` -> trait bounds / trait composition, not subclassing. Record any
  non-trivial inheritance flattening in the Implementation Report.
- `ENUM` -> `enum` (use data-carrying variants where the blueprint implies them).
- `PROPERTY` -> struct field, or getter/setter methods when encapsulation is intended.
- `REFERENCE` / `POINTER` -> borrows (`&T` / `&mut T`) by default; use `Box<T>`,
  `Rc<T>`, or `Arc<T>` only when ownership/sharing semantics clearly require them.
  Do not introduce `unsafe` raw pointers unless the blueprint explicitly requests them.
- `CURRENCY` -> never `f32`/`f64`. Use an integer minor-unit representation or a
  declared decimal crate (e.g. `rust_decimal`) when available; otherwise trigger Step 3.3.
- Error propagation: prefer the `?` operator. Do not add `thiserror` or `anyhow`
  unless declared in `USES`; otherwise define plain `enum` error types in scope.
- `DELEGATE` -> a boxed closure (`Box<dyn Fn(...) -> ...>`) or an `fn` pointer when no capture is needed; use a generic `F: Fn(...)` bound where monomorphization is preferable.

Avoid `unwrap()` and `expect()` in production paths unless the blueprint explicitly permits
panic behavior.

## no_std / embedded_rtos

For `TARGET_PLATFORM = embedded_rtos`:

- use `#![no_std]` when generating crate-level code;
- avoid `std` APIs;
- avoid heap allocations unless an allocator is explicitly specified;
- avoid `Vec`, `String`, `Box`, `HashMap`, and threads unless available by declared crate;
- use `core`, fixed-size arrays, `heapless`, or `arrayvec` only if declared.

If an embedded capability requires a crate not declared in `USES`, trigger Step 3.3.

## Async

Rust async requires a runtime. Do not add `tokio`, `async-std`, or another runtime unless
specified in `USES` or project policy.

If `ASYNC`/`AWAIT` appears with no runtime and the operation requires one, trigger Step 3.3.

## GUI

Rust has no standard GUI library. GUI anchors require `TARGET_UI_FRAMEWORK` and usually a
specified dependency.

Common explicit values:

- `egui`
- `iced`
- `slint`
- `druid`
- `custom:<name>`

## Database

Rust database access requires provider and driver/crate choices.

Examples when explicitly declared:

- SQLite: `rusqlite`, `sqlx` with SQLite feature;
- PostgreSQL: `tokio-postgres`, `postgres`, `sqlx` with Postgres feature;
- MySQL/MariaDB: `mysql`, `sqlx` with MySQL feature.

If `DATABASE_PROVIDER` or driver crate is missing, trigger Step 3.3.
