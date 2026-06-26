# Implementation Report — Python / Tkinter

**Status:** Fully Implemented · **Delivery:** inline · **Indentation:** tabs

## Material decisions
- **TARGET_UI_FRAMEWORK** resolved to **Tkinter** (Python stdlib; no external dependency on `linux`).
- **"correct length"** (unspecified in blueprint) resolved to ISO/IEC 7812 range **13..19 digits**. Overridable.
- **`INT(reversedDigits[i])`** is non-total over arbitrary input; a defensive `isdigit()` guard
  returns `False` for non-numeric content (prevents `ValueError`).

## Realization notes
- `CardInput` numbers-only + live CC formatting on `<KeyRelease>`; caret anchored to digit count
  and restored via `after_idle` (runs after Tk's own key handling) to avoid caret snap-back.
  Re-entrancy guard prevents recursive reformat.
- `PLACEHOLDER` has no native Tkinter equivalent; intent preserved via the instruction label.
- `ShowMessage` -> `tkinter.messagebox.showinfo`. `__main__` guard added.

## Run
    python3 code.py
