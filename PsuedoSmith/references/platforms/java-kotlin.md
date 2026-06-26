# Platform Reference: Java and Kotlin

## Defaults

Java default: Java 17 unless the blueprint specifies another version.
Kotlin default: Kotlin/JVM 1.9 unless the blueprint specifies Android, Native, or another target.

If conservative enterprise compatibility is implied, Java 11 is acceptable and must be
reported.

## Java Rules

- `STRUCT` -> class, record if version supports it and blueprint/style permits.
- `INTERFACE` -> Java interface.
- `LIST` -> `List<T>` / `ArrayList<T>`.
- `COLLECTION` -> `Set<T>` / `HashSet<T>`.
- `DICTIONARY` -> `Map<K,V>` / `HashMap<K,V>`.
- `CURRENCY` -> `java.math.BigDecimal`; never `float`/`double`/`Float`/`Double` for monetary values.
- `DELEGATE` -> a functional interface (`java.util.function.*` or a custom `@FunctionalInterface`); in Kotlin, a function type (e.g. `(T) -> Unit`).
- Always null-check external/database/map results.

Version notes:

- `var` requires Java 10+.
- records require Java 16+.
- virtual threads require Java 21+.
- `HttpClient` requires Java 11+.
- For older versions, generate fallbacks and report them.

## Kotlin Rules

- Prefer `data class` for DTO-like `STRUCT`.
- Use nullable types where values may be absent.
- Do not use `!!` unless the blueprint makes non-null guaranteed.
- Coroutines require `kotlinx.coroutines` unless available in the declared project context.
  Do not add silently.

## GUI

GUI anchors require `TARGET_UI_FRAMEWORK`.

Java examples:

- `Swing` can be used for desktop if explicitly selected.
- `JavaFX` requires an explicit dependency/selection.
- Android UI must not use Swing/AWT.

Kotlin examples:

- Android: `AndroidCompose` or XML layouts only if selected.
- Desktop: Compose Multiplatform, JavaFX, or Swing only if selected/declared.

## Database

JDBC is the common Java/Kotlin database access layer, but drivers are provider-specific.

Require `DATABASE_PROVIDER`. Require `DATABASE_DRIVER` or `USES` for provider drivers such as:

- PostgreSQL JDBC driver;
- MySQL Connector/J;
- SQLite JDBC;
- Microsoft JDBC Driver for SQL Server;
- Oracle JDBC driver.

Do not add ORM frameworks such as Hibernate, JPA, Exposed, or Room unless requested or declared.

## Android Constraints

For `TARGET_PLATFORM = android_arm64`:

- Use Android SDK APIs.
- Do not use desktop GUI APIs.
- Do not perform network or database work on the main thread.
- If storage choice is unclear, trigger Step 3.3.
