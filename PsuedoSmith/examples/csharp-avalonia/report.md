# Implementation Report — C# / Avalonia

**Status:** Partially Implemented (build-complete) · **Delivery:** archive · **Indentation:** spaces:2

## Material decisions
- **TARGET_UI_FRAMEWORK** resolved to **Avalonia 11**. WinForms/WPF rejected at Step 4.1
  (Windows-only vs `linux` target).
- **DELIVERY_MODE: archive** — multi-file build-structured output requires a project layout, so
  inline is not valid; delivered under `code/`.
- **"correct length"** resolved to ISO/IEC 7812 range **13..19 digits**. Overridable.
- Defensive digits-only guard added in `CardValidator.ValidateCardNumber`.

## Realization notes
- Numbers only + live CC formatting via `TextBox.TextChanged` with a re-entrancy guard; caret
  anchored to digit count and re-applied via `Dispatcher.UIThread.Post(..., Background)` so it runs
  after Avalonia's own caret handling (synchronous set caused snap-back).
- `PLACEHOLDER` -> Avalonia **Watermark**. `DISPLAY dialog box` -> custom modal **Window** (no MessageBox).
- File set: `CCValidator.csproj`, `app.manifest`, `Program.cs`, `App.axaml`, `App.axaml.cs`,
  `CardValidator.cs`, `ValidatorWindow.cs`.

## Build & Run (linux)
    cd code
    dotnet restore
    dotnet run

- Requires the .NET 8 SDK (verify with `dotnet --version`). Package versions pinned to 11.0.10.
