# Implementation Report — VBA / LibreOffice Basic

**Status:** Partially Implemented · **Delivery:** inline · **Indentation:** spaces:4

## Material decisions
- **TARGET_UI_FRAMEWORK** resolved to **LibreOffice Basic Dialogs**.
- **"correct length"** resolved to ISO/IEC 7812 range **13..19 digits**. Overridable.
- Defensive digits-only guard added in `ValidateCardNumber`.

## Realization notes
- The dialog **`ccDialog`** (controls: `CardInput`, `ValidateBtn`, `ExitBtn`) must be created in the
  **Dialog Editor** — LibreOffice Basic cannot fully define dialogs in code. Pixel sizes from the
  blueprint map proportionally to dialog map-units.
- Event handlers `OnValidateClicked` / `OnExitClicked` are **assigned manually** on each control's
  Events tab ("Execute action"). This is a manual integration step.
- Live keystroke CC-formatting is not idiomatic for Basic dialogs; digit cleanup is applied at
  validation time via `DigitsOnly`.
- `PLACEHOLDER` -> set as the control's default/help text in the editor.
- `DISPLAY dialog box` -> `MsgBox`.

## Run
- Paste into a module under **Tools ▸ Macros ▸ Edit Macros** (Standard library), build `ccDialog`,
  wire the two handlers, then run `Main`.
